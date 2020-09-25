using SixLabors.Primitives;

namespace SCS.HomePhotos.Service
{
    public interface IImageTransformer
    {
        void ResizeImage(string sourcePath, string savePath, int width, int height, bool useTempFolder = false);
        void ResizeImageByGreatestDimension(string sourcePath, string savePath, int maxHeightOrWidth);
        void ResizeImageByHeight(string sourcePath, string savePath, int height);
        void ResizeImageByWidth(string sourcePath, string savePath, int width);

        (Size original, Size rotated) Rotate(string sourcePath, int angle);

        ImageLayoutInfo GetImageLayoutInfo(string sourcePath);

        void RemoveMetadata(string sourcePath);
    }
}