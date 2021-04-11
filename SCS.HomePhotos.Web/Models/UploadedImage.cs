using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>
    /// Upload file model.
    /// </summary>
    public class UploadedFile
    {
        /// <summary>Gets or sets the tag list.</summary>
        /// <value>The tag list.</value>
        public string TagList { get; set; }

        /// <summary>Gets or sets the files.</summary>
        /// <value>The files.</value>
        public List<IFormFile> Files { get; set; }
    }
}
