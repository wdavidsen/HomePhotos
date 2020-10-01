using MetadataExtractor.Formats.Exif;
using System.Linq;

namespace SCS.HomePhotos.Service
{
    public class ImageMetadataService : IImageMetadataService
    {
        public ExifIfd0Directory GetExifData(string imageFilePath)
        {
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(imageFilePath);
            var exifData = directories.OfType<ExifIfd0Directory>().FirstOrDefault();

            return exifData;
        }
    }
}
