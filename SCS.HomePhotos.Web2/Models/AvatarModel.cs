using Microsoft.AspNetCore.Http;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>The avatar model.</summary>
    public class AvatarModel
    {
        /// <summary>Gets or sets the image.</summary>
        /// <value>The image.</value>
        public IFormFile Image { get; set; }
    }
}
