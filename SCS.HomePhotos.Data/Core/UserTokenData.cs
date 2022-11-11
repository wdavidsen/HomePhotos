using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The user token repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Core.DataBase" />
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IUserTokenData" />
    public class UserTokenData : DataBase, IUserTokenData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTokenData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public UserTokenData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets the user tokens.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        /// <returns>A list of user tokens.</returns>
        public async Task<IEnumerable<UserToken>> GetUserTokens(int userId, bool refresh)
        {
            return await GetListAsync<UserToken>("WHERE UserId = @UserId AND Refresh = @Refresh",
                new { UserId = userId, Refresh = refresh });
        }

        /// <summary>
        /// Deletes a user's refresh token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A void task.</returns>
        public async Task DeleteRefreshToken(int userId, string refreshToken)
        {
            var userTokens = await GetListAsync<UserToken>("WHERE UserId = @UserId AND Refresh = @Refresh AND Token = @Token",
                new { UserId = userId, Refresh = 1, Token = refreshToken });

            foreach (var userToken in userTokens)
            {
                await DeleteAsync(userToken);
            }
        }

        /// <summary>
        /// Deletes the agent refresh tokens.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        public async Task DeleteAgentRefreshTokens(int userId, string agentIdentifier)
        {
            var userTokens = await GetListAsync<UserToken>("WHERE UserId = @UserId AND Refresh = @Refresh AND AgentIdentifier = @AgentIdentifier",
                new { UserId = userId, Refresh = 1, AgentIdentifier = agentIdentifier });

            foreach (var userToken in userTokens)
            {
                await DeleteAsync(userToken);
            }
        }
    }
}
