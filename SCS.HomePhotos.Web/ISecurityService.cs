using System.Collections.Generic;
using System.Security.Claims;

namespace SCS.HomePhotos.Web
{
    public interface ISecurityService
    {
        string ValidIssuer { get; }
        string ValidAudience { get; }

        string GenerateRefreshToken();
        string GenerateToken(IEnumerable<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        List<Claim> GetUserClaims(string userName, bool admin);
    }
}