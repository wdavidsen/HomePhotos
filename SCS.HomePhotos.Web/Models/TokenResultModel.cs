namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Token result model.</summary>
    public class TokenResultModel
    {
        /// <summary>Gets or sets the JWT.</summary>
        /// <value>The JWT.</value>
        public string Jwt { get; set; }

        /// <summary>Gets or sets the refresh token.</summary>
        /// <value>The refresh token.</value>
        public string RefreshToken { get; set; }
    }
}
