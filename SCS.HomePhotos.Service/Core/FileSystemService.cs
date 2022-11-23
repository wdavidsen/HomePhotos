using Microsoft.Extensions.Logging;

using SCS.HomePhotos.Service.Contracts;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// File system service.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Service.Contracts.IFileSystemService" />
    public class FileSystemService : HomePhotosService, IFileSystemService
    {
        private readonly ILogger<FileSystemService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public FileSystemService(ILogger<FileSystemService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The file's checksum.</returns>
        //[SuppressMessage("Security", "SCS0006:Weak hashing function", Justification = "Hash is not being used for security purposes.")]
        public string GetChecksum(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get checksum for file: {filePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The file size.</returns>
        public long GetFileSize(string filePath)
        {
            try
            {
                var fi = new FileInfo(filePath);
                return fi.Length;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get file size for file: {filePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Gets the directory tags.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>A list of tags.</returns>
        public IEnumerable<string> GetDirectoryTags(string filePath)
        {
            var list = new List<string>();

            try
            {
                var dirs = Path.GetDirectoryName(filePath).Split('/', '\\');
                var tag = dirs[^1].Trim();

                list.Add(tag);

                if (dirs.Length > 1)
                {
                    tag = dirs[^2].Trim();

                    if (!list.Contains(tag))
                    {
                        list.Add(tag);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get directory tags for path: {filePath}", filePath);
                throw;
            }

            return list;
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The directory path.</param>
        public void CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create directory at path: {path}", path);
                throw;
            }
        }

        /// <summary>
        /// Deletes the directory files.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="recursive">if set to <c>true</c> delete recursively.</param>
        /// <param name="deleteGuidNamesOnly">if set to <c>true</c> delete only files with GUID names.</param>
        public void DeleteDirectoryFiles(string directoryPath, bool recursive = false, bool deleteGuidNamesOnly = false)
        {
            DeleteDirectoriesAndFiles(directoryPath, recursive, deleteGuidNamesOnly, 0);
        }

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="baseFolder">The base folder.</param>
        /// <param name="subfolderName">Name of the sub-folder.</param>
        /// <param name="fileName">Name of the file.</param>
        public void DeleteImageFile(string baseFolder, string subfolderName, string fileName)
        {
            var fullPath = Path.Combine(baseFolder, subfolderName.Trim('\\'), fileName);
            _logger.LogInformation("Deleting file: {fullPath}.", fullPath);
            File.Delete(fullPath);
        }

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrite destination file.</param>
        public void MoveFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            try
            {
                File.Move(sourcePath, destinationPath, overwrite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to move {sourcePath} to {destinationPath}.", sourcePath, destinationPath);
                throw;
            }
        }

        private void DeleteDirectoriesAndFiles(string directoryPath, bool recursive = false, bool deleteGuidNamesOnly = false, int level = 0)
        {
            if (recursive)
            {
                foreach (var dir in Directory.GetDirectories(directoryPath))
                {
                    DeleteDirectoriesAndFiles(dir, recursive, deleteGuidNamesOnly, level + 1);
                }
            }

            foreach (var file in Directory.GetFiles(directoryPath))
            {
                if (deleteGuidNamesOnly && !Guid.TryParse(Path.GetFileNameWithoutExtension(file), out var g))
                {
                    continue;
                }
                try
                {
                    File.Delete(file);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Failed to delete file: {file}", file);
                }
            }

            if (level > 0 && Directory.GetFiles(directoryPath).Length == 0 && Directory.GetDirectories(directoryPath).Length == 0)
            {
                try
                {
                    Directory.Delete(directoryPath);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Failed to delete directory: {directoryPath}", directoryPath);
                }                
            }      
        }
    }
}
