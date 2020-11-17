﻿using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public class AccountService : IAccountService
    {
        private readonly IStaticConfig _staticConfig;
        private readonly IUserData _userData;
        private readonly IUserTokenData _userTokenData;
        private readonly IAdminLogService _adminLogService;

        public string ValidIssuer => throw new NotImplementedException();

        public AccountService(IStaticConfig staticConfig, IUserData userData, IUserTokenData userTokenData, IAdminLogService adminLogService)
        {
            _staticConfig = staticConfig;
            _userData = userData;
            _userTokenData = userTokenData;
            _adminLogService = adminLogService;
        }

        public async Task<AuthResult> Authenticate(string userName, string password)
        {
            var result = new AuthResult();
            var user = await _userData.GetUser(userName);

            if (user == null)
            {
                result.UserNotExists = true;
                return result;
            }

            if (!user.Enabled)
            {
                result.UserDisabled = true;
            }

            if (!PasswordHash.ValidatePassword(password, user.PasswordHash))
            {
                _adminLogService.LogElevated($"Login for {user.UserName} failed.", LogCategory.Security);
                user.FailedLoginCount++;
                result.AttemptsExceeded = user.FailedLoginCount >= _staticConfig.MaxFailedLogins;

                if (result.AttemptsExceeded)
                {
                    _adminLogService.LogElevated($"Login attempts for {user.UserName} exceeded; account disabled.", LogCategory.Security);
                }

                user.Enabled = user.Enabled && !result.AttemptsExceeded;

                await _userData.UpdateAsync(user);

                result.UserDisabled = !user.Enabled;
                result.PasswordMismatch = true;
                return result;
            }
            else
            {
                _adminLogService.LogNeutral($"Login for {user.UserName} succeeded.", LogCategory.Security);
                user.LastLogin = DateTime.Now;
                user.FailedLoginCount = 0;
                await _userData.UpdateAsync(user);

                result.MustChangePassword = user.MustChangePassword;
                result.User = user;
                return result;
            }            
        }

        public async Task<RegisterResult> Register(User user, string password)
        {
            var result = new RegisterResult();

            var strongPassword = IsStrongPassword(password);

            if (!strongPassword)
            {
                result.PasswordNotStrong = true;
                return result;
            }

            var existingUser = await _userData.GetUser(user.UserName);

            if (existingUser != null)
            {
                result.UserNameTaken = true;
                return result;
            }

            var newUser = new User(user);
            newUser.PasswordHash = PasswordHash.CreateHash(password);
            newUser.Enabled = true;

            await _userData.InsertAsync(newUser);
            _adminLogService.LogNeutral($"New user registration for {user.UserName} succeeded.", LogCategory.Security);

            return result;
        }

        public async Task<ChangePasswordResult> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            var authResult = await Authenticate(userName, currentPassword);

            if (!(authResult.Success || authResult.MustChangePassword))
            {
                _adminLogService.LogElevated($"Password change for {userName} rejected (invalid password).", LogCategory.Security);
                return new ChangePasswordResult(authResult);
            }

            var user = await GetUser(userName, false);
            var passwordHistory = (user.PasswordHistory ?? "").Split('\n');
            var passwordCheckHash = PasswordHash.CreateHashSameSalt(user.PasswordHash, newPassword);

            if (passwordHistory.Any(s => s == passwordCheckHash))
            {
                _adminLogService.LogElevated($"Password change for {userName} rejected (password used previously).", LogCategory.Security);
                return new ChangePasswordResult { PasswordUsedPreviously = true };
            }

            user.PasswordHash = PasswordHash.CreateHash(newPassword);
            user.PasswordHistory += $"\n{passwordCheckHash}";
            user.LastLogin = DateTime.Now;
            user.FailedLoginCount = 0;
            user.MustChangePassword = false;

            await _userData.UpdateAsync(user);
            _adminLogService.LogNeutral($"Password change for {user.UserName} succeeded.", LogCategory.Security);

            return new ChangePasswordResult
            {
                User = user
            };
        }

        public async Task<List<UserToken>> GetRefreshTokens(string userName, string issuer, string audience, string agentIdentifier)
        {
            var tokenList = new List<UserToken>();
            var user = await GetUser(userName);
            var tokens = await _userTokenData.GetUserTokens(user.UserId.Value, true);   

            if (tokens != null && tokens.Count() > 0)
            {
                foreach (var token in tokens)
                {
                    if (token.Refresh && token.AgentIdentifier == agentIdentifier && token.Audience == audience && token.Issuer == issuer)
                    {
                        tokenList.Add(token);
                    }
                }
            }
            return tokenList;
        }

        public async Task DeleteRefreshToken(string userName, string refreshToken)
        {
            var user = await GetUser(userName);

            await _userTokenData.DeleteRefreshToken(user.UserId.Value, refreshToken);
        }

        public async Task DeleteAgentRefreshTokens(string userName, string agentIdentifier)
        {
            var user = await GetUser(userName);

            await _userTokenData.DeleteAgentRefreshTokens(user.UserId.Value, agentIdentifier);
        }

        public async Task SaveRefreshToken(string userName, string newRefreshToken, string agentIdentifier, string issuer, string audience, DateTime expirationUtc)
        {
            var user = await GetUser(userName);

            var userToken = new UserToken
            {
                UserId = user.UserId.Value,
                AgentIdentifier = agentIdentifier,
                Refresh = true,
                Token = newRefreshToken,
                Issuer = issuer,
                Audience = audience,
                Expiration = expirationUtc
            };
            await _userTokenData.InsertAsync(userToken);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _userData.GetListAsync<User>();

            return users;
        }

        public async Task<IEnumerable<User>> GetUsers(RoleType role)
        {
            var users = await _userData.GetListAsync<User>("WHERE Role = @Role", new { Role = role.ToString() });

            return users;
        }

        public async Task<User> GetUser(int userId)
        {
            var user = await _userData.GetAsync<User>(userId);

            return user;
        }

        public async Task<User> GetUser(string userName)
        {
            var user = await _userData.GetUser(userName);

            return user;
        }

        public async Task DeleteUser(int userId)
        {
            var exitingUser = await _userData.GetAsync<User>(userId);
            if (exitingUser == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            await _userData.DeleteAsync(exitingUser);
            _adminLogService.LogNeutral($"User account deletion for {exitingUser.UserName} succeeded.", LogCategory.Security);
        }

        public async Task<User> UpdateAccount(User user)
        {
            var exitingUser = await _userData.GetUser(user.UserName);
            if (exitingUser == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            exitingUser.FirstName = user.FirstName;
            exitingUser.LastName = user.LastName;                        

            await _userData.UpdateAsync(exitingUser);
            _adminLogService.LogNeutral($"User account update for {user.UserName} succeeded.", LogCategory.Security);

            return exitingUser;
        }

        public async Task<User> SaveUser(User user, string password = null)
        {
            if (password != null)
            {
                user.PasswordHash = PasswordHash.CreateHash(password);
            }

            if (user.UserId > 0)
            {
                var exitingUser = await _userData.GetAsync<User>(user.UserId.Value);
                if (exitingUser == null)
                {
                    throw new InvalidOperationException("User does not exist.");
                }

                exitingUser.FirstName = user.FirstName;
                exitingUser.LastName = user.LastName;
                exitingUser.Enabled = user.Enabled;
                exitingUser.Role = user.Role;
                exitingUser.MustChangePassword = user.MustChangePassword;

                await _userData.UpdateAsync(exitingUser);
                _adminLogService.LogNeutral($"User account update for {user.UserName} succeeded.", LogCategory.Security);
            }
            else
            {
                var userId = await _userData.InsertAsync(user);
                _adminLogService.LogNeutral($"User account creation for {user.UserName} succeeded.", LogCategory.Security);
                user.UserId = userId.Value;
            }
            
            return user;
        }

        protected async Task<User> GetUser(string userName, bool throwIfNotExists = true)
        {
            var user = await _userData.GetUser(userName);

            if (user == null && throwIfNotExists)
            {
                throw new InvalidOperationException($"User does not exist: {userName}.");
            }

            return user;
        }

        private bool IsStrongPassword(string password)
        {
            if (password.Length < _staticConfig.PasswordRequirements.MinLength)
            {
                return false;
            }
            GetCharCounts(password, out int specialCharaterCount, out int digitCount, out int uppercaseLetterCount);

            return specialCharaterCount >= _staticConfig.PasswordRequirements.SpecialCharacters 
                && digitCount >= _staticConfig.PasswordRequirements.Digits 
                && uppercaseLetterCount >= _staticConfig.PasswordRequirements.UppercaseCharacters;
        }

        private void GetCharCounts(string s, out int specialCharacterCount, out int digitCount, out int capitalLetterCount)
        {
            specialCharacterCount = 0;
            digitCount = 0;
            capitalLetterCount = 0;

            foreach (var c in s.ToCharArray())
            {
                if (char.IsDigit(c))
                {
                    digitCount++;
                }
                else if (char.IsLetter(c) && char.IsUpper(c))
                {
                    capitalLetterCount++;
                }
                else if (!(char.IsLetter(c) || char.IsNumber(c)))
                {
                    specialCharacterCount++;
                }
            }
        }
    }
}
