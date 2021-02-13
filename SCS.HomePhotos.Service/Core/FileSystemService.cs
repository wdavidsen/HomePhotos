using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace SCS.HomePhotos.Service.Core
{
    public class FileSystemService : IFileSystemService
    {
        private readonly ILogger<FileSystemService> _logger;

        public FileSystemService(ILogger<FileSystemService> logger)
        {
            _logger = logger;
        }

        [SuppressMessage("Security", "SCS0006:Weak hashing function", Justification = "Hash is not being used for security purposes.")]
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

        public IEnumerable<string> GetDirectoryTags(string filePath)
        {
            var list = new List<string>();

            try
            {
                var dirs = Path.GetDirectoryName(filePath).Split('/', '\\');
                var tag = dirs[dirs.Length - 1];

                list.Add(tag);

                if (dirs.Length > 1)
                {
                    tag = dirs[dirs.Length - 2];

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

        public void DeleteDirectoryFiles(string directoryPath, bool recursive = true)
        {
            try
            {
                foreach (var subdir in Directory.GetDirectories(directoryPath))
                {
                    var deletePath = subdir.TrimEnd('/', '\\') + "_ToDelete";

                    Directory.Move(subdir, deletePath);
                    Directory.Delete(deletePath, recursive);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete directory: {directoryPath}", directoryPath);
                throw;
            }
        }

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
    }
}
