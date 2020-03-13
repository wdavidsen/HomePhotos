using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web
{
    public class FileUploadService : IFileUploadService
    {
        public async Task CopyFile(IFormFile formFile, string filePath, FileMode fileMode = FileMode.Create)
        {
            using (var stream = new FileStream(filePath, fileMode))
            {
                await formFile.CopyToAsync(stream);
            }
        }
    }
}
