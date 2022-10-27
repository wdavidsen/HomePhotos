using Microsoft.IdentityModel.Tokens;
using System;

namespace SCS.HomePhotos.Web.Security
{
    /// <summary>
    /// A wrapper for JWT security info.
    /// </summary>
    public class JwtAuthentication
    {
        /// <summary>
        /// Gets or sets the security key.
        /// </summary>
        /// <value>
        /// The security key.
        /// </value>
        public string SecurityKey { get; set; }

        /// <summary>
        /// Gets or sets the valid issuer.
        /// </summary>
        /// <value>
        /// The valid issuer.
        /// </value>
        public string ValidIssuer { get; set; }

        /// <summary>
        /// Gets or sets the valid audience.
        /// </summary>
        /// <value>
        /// The valid audience.
        /// </value>
        public string ValidAudience { get; set; }

        /// <summary>
        /// Gets the symmetric security key.
        /// </summary>
        /// <value>
        /// The symmetric security key.
        /// </value>
        public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Convert.FromBase64String(SecurityKey));

        /// <summary>
        /// Gets the signing credentials.
        /// </summary>
        /// <value>
        /// The signing credentials.
        /// </value>
        public SigningCredentials SigningCredentials => new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }
}
