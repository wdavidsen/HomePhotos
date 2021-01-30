using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SCS.HomePhotos.Web
{
    public interface IFileUploadService
    {
        Task CopyFile(IFormFile formFile, string filePath, FileMode fileMode = FileMode.Create);

        void CreateDirectory(string directoryPath);
    }
}