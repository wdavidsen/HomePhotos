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
