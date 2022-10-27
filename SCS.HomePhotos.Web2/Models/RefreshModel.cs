using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Token refresh model.</summary>
    public class RefreshModel
    {
        /// <summary>Gets or sets the JWT.</summary>
        /// <value>The JWT.</value>
        [Required]
        public string Jwt { get; set; }

        /// <summary>Gets or sets the refresh token.</summary>
        /// <value>The refresh token.</value>
        [Required]
        public string RefreshToken { get; set; }
    }
}
