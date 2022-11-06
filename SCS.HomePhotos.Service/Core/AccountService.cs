using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Account services.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Service.Contracts.IAccountService" />
    public class AccountService : IAccountService
    {
        private readonly IStaticConfig _staticConfig;
        private readonly IUserData _userData;
        private readonly IUserTokenData _userTokenData;
        private readonly IAdminLogService _adminLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        /// <param name="userData">The user data.</param>
        /// <param name="userTokenData">The user token data.</param>
        /// <param name="adminLogService">The admin log service.</param>
        public AccountService(IStaticConfig staticConfig, IUserData userData, IUserTokenData userTokenData, IAdminLogService adminLogService)
        {
            _staticConfig = staticConfig;
            _userData = userData;
            _userTokenData = userTokenData;
            _adminLogService = adminLogService;
        }

        /// <summary>
        /// Authenticates a specified user.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>An authentication result.</returns>
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

        /// <summary>
        /// Registers a specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns>A registration result.</returns>
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
            newUser.Enabled = false;

            await _userData.InsertAsync(newUser);
            _adminLogService.LogNeutral($"New user registration for {user.UserName} succeeded.", LogCategory.Security);

            return result;
        }

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The change password result.</returns>
        public async Task<ChangePasswordResult> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            var result = new ChangePasswordResult(await Authenticate(userName, currentPassword));

            if (!(result.Success || result.MustChangePassword))
            {
                _adminLogService.LogElevated($"Password change for {userName} rejected (invalid password).", LogCategory.Security);
                return new ChangePasswordResult(result);
            }

            var strongPassword = IsStrongPassword(newPassword);

            if (!strongPassword)
            {
                result.PasswordNotStrong = true;
                return result;
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

        /// <summary>
        /// Changes a user's password. Allows an admin to resets a user's password without a current password.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The change password result.</returns>
        public async Task<ChangePasswordResult> ResetPassword(string userName, string newPassword)
        {
            var strongPassword = IsStrongPassword(newPassword);
            var result = new ChangePasswordResult();

            if (!strongPassword)
            {
                result.PasswordNotStrong = true;
                return result;
            }

            var user = await GetUser(userName, false);
            var passwordCheckHash = PasswordHash.CreateHashSameSalt(user.PasswordHash, newPassword);

            user.PasswordHash = PasswordHash.CreateHash(newPassword);
            user.PasswordHistory += $"\n{passwordCheckHash}";
            user.LastLogin = DateTime.Now;
            user.FailedLoginCount = 0;
            user.MustChangePassword = false;

            await _userData.UpdateAsync(user);
            result.User = user;
            _adminLogService.LogNeutral($"Password reset for {user.UserName} succeeded.", LogCategory.Security);

            return result;
        }

        /// <summary>
        /// Gets the refresh tokens for a use.
        /// </summary>
        /// <param name="userName">Usename of the user.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        /// <returns>A list of tokens.</returns>
        public async Task<List<UserToken>> GetRefreshTokens(string userName, string issuer, string audience, string agentIdentifier)
        {
            var tokenList = new List<UserToken>();
            var user = await GetUser(userName);

            if (user != null)
            {
                var tokens = await _userTokenData.GetUserTokens(user.UserId.Value, true);

                if (tokens != null && tokens.Any())
                {
                    foreach (var token in tokens)
                    {
                        if (token.Refresh && token.AgentIdentifier == agentIdentifier && token.Audience == audience && token.Issuer == issuer)
                        {
                            tokenList.Add(token);
                        }
                    }
                }
            }
            return tokenList;
        }

        /// <summary>
        /// Deletes a user's refresh token.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A void task.</returns>
        public async Task DeleteRefreshToken(string userName, string refreshToken)
        {
            var user = await GetUser(userName);

            await _userTokenData.DeleteRefreshToken(user.UserId.Value, refreshToken);
        }

        /// <summary>
        /// Deletes the agent refresh tokens.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        /// <returns>A void task.</returns>
        public async Task DeleteAgentRefreshTokens(string userName, string agentIdentifier)
        {
            var user = await GetUser(userName);

            await _userTokenData.DeleteAgentRefreshTokens(user.UserId.Value, agentIdentifier);
        }

        /// <summary>
        /// Saves a user refresh token.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="newRefreshToken">The new refresh token.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="expirationUtc">The expiration UTC.</param>
        /// <returns>A void task.</returns>
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

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _userData.GetListAsync<User>();

            return users;
        }

        /// <summary>
        /// Gets a list of users by role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>A list of matching users.</returns>
        public async Task<IEnumerable<User>> GetUsers(RoleType role)
        {
            var users = await _userData.GetListAsync<User>("WHERE Role = @Role", new { Role = role.ToString() });

            return users;
        }

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user entity.</returns>
        public async Task<User> GetUser(int userId)
        {
            var user = await _userData.GetAsync<User>(userId);

            return user;
        }

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="userName">The user identifier.</param>
        /// <returns>The user entity.</returns>
        public async Task<User> GetUser(string userName)
        {
            var user = await _userData.GetUser(userName);

            return user;
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A void task.</returns>
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

        /// <summary>
        /// Updates a user's account information.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <returns>The user entity.</returns>
        public async Task<User> UpdateAccount(User user)
        {
            var exitingUser = await _userData.GetUser(user.UserName);
            if (exitingUser == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            exitingUser.FirstName = user.FirstName;
            exitingUser.LastName = user.LastName;
            exitingUser.EmailAddress = user.EmailAddress;

            await _userData.UpdateAsync(exitingUser);
            _adminLogService.LogNeutral($"User account update for {user.UserName} succeeded.", LogCategory.Security);

            return exitingUser;
        }

        /// <summary>
        /// Saves a user.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user entity.</returns>
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
                exitingUser.EmailAddress = user.EmailAddress;
                exitingUser.AvatarImage = user.AvatarImage;
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

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="throwIfNotExists">if set to <c>true</c>[throw exceptin if not exists.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">User does not exist: {userName}.</exception>
        protected async Task<User> GetUser(string userName, bool throwIfNotExists = true)
        {
            var user = await _userData.GetUser(userName);

            if (user == null && throwIfNotExists)
            {
                throw new InvalidOperationException($"User does not exist: {userName}.");
            }

            return user;
        }

        /// <summary>
        /// Determines whether password is strong.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>
        ///   <c>true</c> if password is strong; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Gets the password character counts.
        /// </summary>
        /// <param name="password">The password to test.</param>
        /// <param name="specialCharacterCount">The special character count.</param>
        /// <param name="digitCount">The digit count.</param>
        /// <param name="capitalLetterCount">The capital letter count.</param>
        private static void GetCharCounts(string password, out int specialCharacterCount, out int digitCount, out int capitalLetterCount)
        {
            specialCharacterCount = 0;
            digitCount = 0;
            capitalLetterCount = 0;

            foreach (var c in password.ToCharArray())
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
