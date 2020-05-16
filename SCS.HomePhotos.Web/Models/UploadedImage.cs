using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SCS.HomePhotos.Web.Models
{
    public class UploadedFile
    {
        public string TagList { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
