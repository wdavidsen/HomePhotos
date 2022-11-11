using Microsoft.Net.Http.Headers;

namespace SCS.HomePhotos.Web
{
    /// <summary>
    /// Extensions methods to assist with file uploading.
    /// </summary>
    public static class IFormFileExtensions
    {
        /// <summary>
        /// Gets the name of the uploaded file.
        /// </summary>
        /// <param name="file">The file name.</param>
        /// <returns>The file name.</returns>
        public static string GetFileName(this IFormFile file)
        {
            return ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');
        }

        /// <summary>
        /// Gets the uploaded file extension.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The file extension.</returns>
        public static string GetFileExtension(this IFormFile file)
        {
            return Path.GetExtension(GetFileName(file));
        }

        /// <summary>
        /// Gets the uploaded file stream.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A file steam.</returns>
        public static async Task<MemoryStream> GetFileStream(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream;
        }

        /// <summary>
        /// Gets the file bytes.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A byte array.</returns>
        public static async Task<byte[]> GetFileArray(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream.ToArray();
        }
    }
}
