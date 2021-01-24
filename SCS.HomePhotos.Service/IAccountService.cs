using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Service
{
    public interface IAccountService
    {
        Task<AuthResult> Authenticate(string userName, string password);
        Task<RegisterResult> Register(User user, string password);
        Task<List<UserToken>> GetRefreshTokens(string userName, string issuer, string audience, string agentIdentifier);
        Task DeleteRefreshToken(string userName, string refreshToken);
        Task DeleteAgentRefreshTokens(string userName, string agentIdentifier);
        Task SaveRefreshToken(string userName, string newRefreshToken, string agentIdentifier, string issuer, string audience, DateTime expirationUtc);
        Task<ChangePasswordResult> ChangePassword(string userName, string currentPassword, string newPassword);
        Task<User> ResetPassword(string userName, string newPassword);
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<User>> GetUsers(RoleType role);
        Task<User> GetUser(int userId);
        Task<User> GetUser(string userId);
        Task DeleteUser(int userId);
        Task<User> SaveUser(User user, string password = null);

        Task<User> UpdateAccount(User user);
    }
}