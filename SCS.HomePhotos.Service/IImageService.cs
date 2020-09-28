using MetadataExtractor.Formats.Exif;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public interface IImageService
    {
        Task<string> QueueMobileResize(string imageFilePath, bool copyToTempFolder = true, params string[] tags);

        string CreateCachePath(string checksum, string extension);

        string GetFullCachePath(string cachePath, ImageSizeType imageType);

        string CreateSmallImage(string imageFilePath, string cacheSubfolder);

        string CreateFullImage(string imageFilePath, string cacheSubfolder);

        string CreateThumbnail(string imageFilePath, string cacheSubfolder);

        void OrientImage(string imageFilePath, ExifIfd0Directory exifData);

        Photo SavePhotoAndTags(Photo existingPhoto, string imageFilePath, string checksum, string cacheSubfolder, 
            ImageLayoutInfo imageLayoutInfo, ExifIfd0Directory exifData, params string[] tags);

        ImageLayoutInfo GetImageLayoutInfo(string sourcePath);

        ImageInfo GetImageInfo(ExifIfd0Directory exifData);

        Dictionary<string, string> GetImageMetadata(string imageFilePath);
    }
}