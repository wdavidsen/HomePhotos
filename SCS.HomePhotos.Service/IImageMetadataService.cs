using MetadataExtractor.Formats.Exif;

namespace SCS.HomePhotos.Service
{
    public interface IImageMetadataService
    {
        ExifIfd0Directory GetExifData(string imageFilePath);
    }
}