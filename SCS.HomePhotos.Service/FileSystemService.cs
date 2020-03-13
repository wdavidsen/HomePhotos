using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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

        public ImageInfo GetInfo(string filePath)
        {
            var imageInfo = new ImageInfo();

            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(filePath);
            var exifData = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            if (exifData != null)
            {
                if (exifData.HasTagName(ExifDirectoryBase.TagDateTimeOriginal))
                {
                    var dateTaken = exifData?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);
                    var dateParts = dateTaken.Split(':', '-', '.', ' ', 'T');
                    imageInfo.DateTaken = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]),
                        int.Parse(dateParts[3]), int.Parse(dateParts[4]), int.Parse(dateParts[5]));
                }

                var exifTag = exifData.GetDescription(ExifDirectoryBase.TagModel);

                if (exifTag != null)
                {
                    imageInfo.Tags.Add(exifTag);
                }
            }

            var dirs = Path.GetDirectoryName(filePath).Split('/', '\\');
            var tag = dirs[dirs.Length - 1];

            imageInfo.Tags.Add(tag);

            if (dirs.Length > 1)
            {
                tag = dirs[dirs.Length - 2];

                if (!imageInfo.Tags.Contains(tag))
                {
                    imageInfo.Tags.Add(tag);
                }
            }

            return imageInfo;
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
