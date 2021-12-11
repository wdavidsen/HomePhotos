using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SCS.HomePhotos.Web
{
    /// <summary>
    /// Provides file upload services.
    /// </summary>
    public interface IFileUploadService
    {
        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="formFile">The form file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <returns>A void task.</returns>
        Task CopyFile(IFormFile formFile, string filePath, FileMode fileMode = FileMode.Create);

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        void CreateDirectory(string directoryPath);
    }
}