using MetadataExtractor.Formats.Exif;
using SCS.HomePhotos.Service.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Image metadata service.
    /// </summary>
    public class ImageMetadataService : IImageMetadataService
    {
        /// <summary>
        /// Gets the EXIF data.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <returns>A list of EXIF data.</returns>
        public IEnumerable<ExifDirectoryBase> GetExifData(string imageFilePath)
        {
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(imageFilePath);
            var exifData1 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            var exifData2 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();

            var list = new List<ExifDirectoryBase>();

            if (exifData1 != null)
            {
                list.Add(exifData1);
            }

            if (exifData2 != null)
            {
                list.Add(exifData2);
            }

            return list;            
        }
    }
}
