using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    public class UserTokenData : DataBase, IUserTokenData
    {
        public UserTokenData(IStaticConfig staticConfig) : base(staticConfig) { }

        public async Task<IEnumerable<UserToken>> GetUserTokens(int userId, bool refresh)
        {
            return await GetListAsync<UserToken>("WHERE UserId = @UserId AND Refresh = @Refresh",
                new { UserId = userId, Refresh = refresh});
        }

        public async Task DeleteRefreshToken(int userId, string refreshToken)
        {
            var userTokens = await GetListAsync<UserToken>("WHERE UserId = @UserId AND Refresh = @Refresh AND Token = @Token",
                new { UserId = userId, Refresh = 1, Token = refreshToken });

            foreach (var userToken in userTokens)
            {
                await DeleteAsync(userToken);
            }
        }

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
