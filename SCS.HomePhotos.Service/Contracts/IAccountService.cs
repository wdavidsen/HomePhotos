using SCS.HomePhotos.Model;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Account services.
    /// </summary>
    public interface IAccountService : IHomePhotosService
    {
        /// <summary>
        /// Authenticates a specified user.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>An authentication result.</returns>
        Task<AuthResult> Authenticate(string userName, string password);

        /// <summary>
        /// Registers a specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns>A registration result.</returns>
        Task<RegisterResult> Register(User user, string password);

        /// <summary>
        /// Gets the refresh tokens for a use.
        /// </summary>
        /// <param name="userName">Usename of the user.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        /// <returns>A list of tokens.</returns>
        Task<List<UserToken>> GetRefreshTokens(string userName, string issuer, string audience, string agentIdentifier);

        /// <summary>
        /// Deletes a user's refresh token.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A void task.</returns>
        Task DeleteRefreshToken(string userName, string refreshToken);

        /// <summary>
        /// Deletes the agent refresh tokens.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        /// <returns>A void task.</returns>
        Task DeleteAgentRefreshTokens(string userName, string agentIdentifier);

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
        Task SaveRefreshToken(string userName, string newRefreshToken, string agentIdentifier, string issuer, string audience, DateTime expirationUtc);

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The change password result.</returns>
        Task<ChangePasswordResult> ChangePassword(string userName, string currentPassword, string newPassword);

        /// <summary>
        /// Changes a user's password. Allows an admin to resets a user's password without a current password.
        /// </summary>
        /// <param name="userName">Username of the user.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The change password result.</returns>
        Task<ChangePasswordResult> ResetPassword(string userName, string newPassword);

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        Task<IEnumerable<User>> GetUsers();

        /// <summary>
        /// Gets a list of users by role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>A list of matching users.</returns>
        Task<IEnumerable<User>> GetUsers(RoleType role);

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user entity.</returns>
        Task<User> GetUser(int userId);

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user entity.</returns>
        Task<User> GetUser(string userId);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A void task.</returns>
        Task DeleteUser(int userId);

        /// <summary>
        /// Saves a user.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user entity.</returns>
        Task<User> SaveUser(User user, string password = null);

        /// <summary>
        /// Updates a user's account information.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <returns>The user entity.</returns>
        Task<User> UpdateAccount(User user);
    }
}