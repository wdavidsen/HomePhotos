using SCS.HomePhotos.Model;

using System.Security.Claims;

namespace SCS.HomePhotos.Web
{
    /// <summary>
    /// Provides security services.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Web.ISecurityService" />
    public interface ISecurityService
    {
        /// <summary>
        /// Gets the valid issuer.
        /// </summary>
        /// <value>
        /// The valid issuer.
        /// </value>
        string ValidIssuer { get; }

        /// <summary>
        /// Gets the valid audience.
        /// </summary>
        /// <value>
        /// The valid audience.
        /// </value>
        string ValidAudience { get; }

        /// <summary>
        /// Generates a refresh token.
        /// </summary>
        /// <returns>The refresh token.</returns>
        string GenerateRefreshToken();


        /// <summary>
        /// Generates a JWT security token.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <returns>A JWT security token</returns>
        string GenerateToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Gets the principal from expired token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="Microsoft.IdentityModel.Tokens.SecurityTokenException">
        /// Invalid token
        /// or
        /// Invalid token
        /// </exception>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        /// <summary>
        /// Gets a user's claims.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The user role.</param>
        /// <returns>A list of claims.</returns>
        List<Claim> GetUserClaims(Model.User user, RoleType role);
    }
}