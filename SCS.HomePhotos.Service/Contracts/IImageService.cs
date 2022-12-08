using MetadataExtractor.Formats.Exif;
using SCS.HomePhotos.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Image service.
    /// </summary>
    public interface IImageService : IHomePhotosService
    {
        /// <summary>
        /// Queues the image to be resized for mobile display.
        /// </summary>
        /// <param name="uploadedBy">User that uploaded file.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The cached file path.</returns>
        Task<string> QueueMobileResize(User uploadedBy, string imageFilePath, List<Tag> tags);

        /// <summary>
        /// Creates the cache path.
        /// </summary>
        /// <param name="checksum">The checksum.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>The image cache path.</returns>
        string CreateCachePath(string checksum, string extension);

        /// <summary>
        /// Gets the full-sized image cache path.
        /// </summary>
        /// <param name="cachePath">The cache path.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <returns>The full-sized image cache path.</returns>
        string GetFullCachePath(string cachePath, ImageSizeType imageType);

        /// <summary>
        /// Creates the small-sized image.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cacheSubfolder">The cache subfolder.</param>
        /// <returns>The small-sized image cache path.</returns>
        string CreateSmallImage(string imageFilePath, string cacheSubfolder);

        /// <summary>
        /// Creates the full-sized image.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cacheSubfolder">The cache subfolder.</param>
        /// <returns>The full-sized image cache path.</returns>
        string CreateFullImage(string imageFilePath, string cacheSubfolder);

        /// <summary>
        /// Creates the thumbnail-sized image.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cacheSubfolder">The cache subfolder.</param>
        /// <returns>The thumbnail-sized image cache path.</returns>
        string CreateThumbnail(string imageFilePath, string cacheSubfolder);

        /// <summary>
        /// Orients the image for proper viewing.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="exifDataList">The EXIF data list.</param>
        void OrientImage(string imageFilePath, IEnumerable<ExifDirectoryBase> exifDataList);

        /// <summary>
        /// Saves the photo and tags.
        /// </summary>
        /// <param name="existingPhoto">The existing photo.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="checksum">The checksum.</param>
        /// <param name="cacheSubfolder">The cache subfolder.</param>
        /// <param name="imageLayoutInfo">The image layout information.</param>
        /// <param name="imageInfo">The image metadata.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The photo object.</returns>
        Photo SavePhotoAndTags(Photo existingPhoto, string imageFilePath, string checksum, string cacheSubfolder,
            ImageLayoutInfo imageLayoutInfo, ImageInfo imageInfo, List<Tag> tags);

        /// <summary>
        /// Gets the image layout information.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>The image layout information.</returns>
        ImageLayoutInfo GetImageLayoutInfo(string sourcePath);

        /// <summary>
        /// Gets basic image information.
        /// </summary>
        /// <param name="exifDataList">The EXIF data list.</param>
        /// <returns>Basic image information.</returns>
        ImageInfo GetImageInfo(IEnumerable<ExifDirectoryBase> exifDataList);
    }
}