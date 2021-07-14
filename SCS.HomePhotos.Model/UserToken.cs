using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The user token entity.
    /// </summary>
    [Table("UserToken")]
    public class UserToken
    {
        /// <summary>
        /// Gets or sets the token identifier.
        /// </summary>
        /// <value>
        /// The token identifier.
        /// </value>
        [Key]
        public int TokenId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether token is a refresh token.
        /// </summary>
        /// <value>
        ///   <c>true</c> if refresh; otherwise, <c>false</c>.
        /// </value>
        public bool Refresh { get; set; }

        /// <summary>
        /// Gets or sets the token issuer.
        /// </summary>
        /// <value>
        /// The token issuer.
        /// </value>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the token audience.
        /// </summary>
        /// <value>
        /// The token audience.
        /// </value>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the token expiration.
        /// </summary>
        /// <value>
        /// The token expiration.
        /// </value>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets the agent identifier.
        /// </summary>
        /// <value>
        /// The agent identifier.
        /// </value>
        public string AgentIdentifier { get; set; }
    }
}
