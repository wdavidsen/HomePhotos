using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The user token repository.
    /// </summary>
    public interface IUserTokenData : IDataBase<UserToken>
    {
        /// <summary>
        /// Deletes a user's refresh token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A void task.</returns>
        Task DeleteRefreshToken(int userId, string refreshToken);

        /// <summary>
        /// Gets the user tokens.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        /// <returns>A list of user tokens.</returns>
        Task<IEnumerable<UserToken>> GetUserTokens(int userId, bool refresh);


        /// <summary>
        /// Deletes a user's tokens for a specific user agent.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        /// <returns>A void task.</returns>
        Task DeleteAgentRefreshTokens(int userId, string agentIdentifier);
    }
}