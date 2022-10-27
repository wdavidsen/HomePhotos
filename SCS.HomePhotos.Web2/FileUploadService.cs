using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web
{
    /// <summary>
    /// Provides file uploading services.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Web.IFileUploadService" />
    public class FileUploadService : IFileUploadService
    {
        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="formFile">The form file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <returns>A void task.</returns>
        public async Task CopyFile(IFormFile formFile, string filePath, FileMode fileMode = FileMode.Create)
        {
            using (var stream = new FileStream(filePath, fileMode))
            {
                await formFile.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        public void CreateDirectory(string directoryPath)
        {
            if (Path.HasExtension(directoryPath))
            {
                directoryPath = Path.GetDirectoryName(directoryPath);
            }
            Directory.CreateDirectory(directoryPath);
        }
    }
}
