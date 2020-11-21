using MetadataExtractor.Formats.Exif;
using System.Linq;

namespace SCS.HomePhotos.Service
{
    public class ImageMetadataService : IImageMetadataService
    {
        public ExifSubIfdDirectory GetExifData(string imageFilePath)
        {
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(imageFilePath);
            var exifData = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();            
            return exifData;
        }
    }
}
