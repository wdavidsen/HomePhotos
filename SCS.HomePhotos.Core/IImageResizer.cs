namespace SCS.HomePhotos.Service
{
    public interface IImageResizer
    {
        void ResizeImage(string sourcePath, string savePath, int width, int height, bool useTempFolder = false);
        void ResizeImageByGreatestDimension(string sourcePath, string savePath, int maxHeightOrWidth);
        void ResizeImageByHeight(string sourcePath, string savePath, int height);
        void ResizeImageByWidth(string sourcePath, string savePath, int width);

        ImageLayoutInfo GetImageLayoutInfo(string sourcePath);
    }
}