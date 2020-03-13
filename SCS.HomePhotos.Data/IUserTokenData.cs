using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public interface IUserTokenData : IDataBase
    {
        Task DeleteRefreshToken(int userId, string refreshToken);
        Task<IEnumerable<UserToken>> GetUserTokens(int userId, bool refresh);
        Task DeleteAgentRefreshTokens(int userId, string agentIdentifier);
    }
}