using MetadataExtractor.Formats.Exif;

namespace SCS.HomePhotos.Service
{
    public interface IImageMetadataService
    {
        ExifSubIfdDirectory GetExifData(string imageFilePath);
    }
}