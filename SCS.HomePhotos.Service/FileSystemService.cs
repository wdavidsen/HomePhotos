using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace SCS.HomePhotos.Service
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
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public long GetFileSize(string filePath)
        {
            var fi = new FileInfo(filePath);
            return fi.Length;
        }

        public IEnumerable<string> GetDirectoryTags(string filePath)
        {
            var list = new List<string>();

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

            return list;
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectoryFiles(string cacheFolder, bool recursive = true)
        {
            foreach (var subdir in Directory.GetDirectories(cacheFolder))
            {
                var deletePath = subdir.TrimEnd('/', '\\') + "_ToDelete";

                Directory.Move(subdir, deletePath);
                Directory.Delete(deletePath, recursive);
            }
        }
    }
}
