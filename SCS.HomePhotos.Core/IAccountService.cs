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
    }
}