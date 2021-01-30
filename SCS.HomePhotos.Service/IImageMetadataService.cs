using MetadataExtractor.Formats.Exif;
using System.Collections.Generic;

namespace SCS.HomePhotos.Service
{
    public interface IImageMetadataService
    {
        IEnumerable<ExifDirectoryBase> GetExifData(string imageFilePath);
    }
}