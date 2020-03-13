using SCS.HomePhotos.Model;
using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public interface IImageService
    {
        Task<string> QueueMobileResize(string imageFilePath, bool copyToTempFolder = true);

        string CreateCachePath(string checksum, string extension);

        string GetFullCachePath(string cachePath, ImageSizeType imageType);

        string CreateSmallImage(string imageFilePath, string cacheSubfolder);

        string CreateFullImage(string imageFilePath, string cacheSubfolder);

        string CreateThumbnail(string imageFilePath, string cacheSubfolder);

        Photo SavePhotoAndTags(string imageFilePath, string checksum, string cacheSubfolder);

        ImageLayoutInfo GetImageLayoutInfo(string sourcePath);
    }
}