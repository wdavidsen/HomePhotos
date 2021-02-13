using MetadataExtractor.Formats.Exif;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{
    public interface IImageService
    {
        Task<string> QueueMobileResize(string contextUserName, string imageFilePath,  params string[] tags);

        string CreateCachePath(string checksum, string extension);

        string GetFullCachePath(string cachePath, ImageSizeType imageType);

        string CreateSmallImage(string imageFilePath, string cacheSubfolder);

        string CreateFullImage(string imageFilePath, string cacheSubfolder);

        string CreateThumbnail(string imageFilePath, string cacheSubfolder);

        void OrientImage(string imageFilePath, IEnumerable<ExifDirectoryBase> exifDataList);

        Photo SavePhotoAndTags(Photo existingPhoto, string imageFilePath, string checksum, string cacheSubfolder, 
            ImageLayoutInfo imageLayoutInfo, IEnumerable<ExifDirectoryBase> exifDataList, params string[] tags);

        ImageLayoutInfo GetImageLayoutInfo(string sourcePath);

        ImageInfo GetImageInfo(IEnumerable<ExifDirectoryBase> exifDataList);
    }
}