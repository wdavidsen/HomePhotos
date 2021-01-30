using MetadataExtractor.Formats.Exif;
using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Service
{
    public class ImageMetadataService : IImageMetadataService
    {
        public IEnumerable<ExifDirectoryBase> GetExifData(string imageFilePath)
        {
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(imageFilePath);
            var exifData1 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            var exifData2 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();

            return new List<ExifDirectoryBase> { exifData1 , exifData2 };
        }
    }
}
