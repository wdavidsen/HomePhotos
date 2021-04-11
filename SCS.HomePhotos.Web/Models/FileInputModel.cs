using Microsoft.AspNetCore.Http;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>
    /// File upload model.
    /// </summary>
    public class FileInputModel
    {
        /// <summary>
        /// Gets or sets the file to upload.
        /// </summary>
        /// <value>
        /// The file to upload.
        /// </value>
        public IFormFile FileToUpload { get; set; }
    }
}
