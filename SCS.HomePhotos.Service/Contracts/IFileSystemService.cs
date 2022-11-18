using System.Collections.Generic;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// File system service.
    /// </summary>
    public interface IFileSystemService : IHomePhotosService
    {
        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The directory path.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        string GetChecksum(string filePath);

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        long GetFileSize(string filePath);

        /// <summary>
        /// Gets the directory tags.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>A list of tags.</returns>
        IEnumerable<string> GetDirectoryTags(string filePath);

        /// <summary>
        /// Deletes the directory files.
        /// </summary>
        /// <param name="cacheFolder">The cache folder.</param>
        /// <param name="recursive">if set to <c>true</c> delete files recursivly.</param>
        void DeleteDirectoryFiles(string cacheFolder, bool recursive = true);

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="baseFolder">The base folder.</param>
        /// <param name="subfolderName">Name of the subfolder.</param>
        /// <param name="fileName">Name of the file.</param>
        void DeleteImageFile(string baseFolder, string subfolderName, string fileName);

        /// <summary>
        /// Moves the file.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrite destination file.</param>
        void MoveFile(string sourcePath, string destinationPath, bool overwrite = false);
    }
}