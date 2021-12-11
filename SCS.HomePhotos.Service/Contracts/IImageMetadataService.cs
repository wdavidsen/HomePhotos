using MetadataExtractor.Formats.Exif;
using System.Collections.Generic;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Image metadata service.
    /// </summary>
    public interface IImageMetadataService
    {
        /// <summary>
        /// Gets the EXIF data.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <returns>A list of EXIF data.</returns>
        IEnumerable<ExifDirectoryBase> GetExifData(string imageFilePath);
    }
}