using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    public class RefreshModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
