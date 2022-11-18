using SixLabors.ImageSharp;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Helper class for common image operations.
    /// </summary>
    public interface IImageTransformer : IHomePhotosService
    {
        /// <summary>
        /// Resizes an image.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="savePath">The save path.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="useTempFolder">if set to <c>true</c> use temporary folder.</param>
        void ResizeImage(string sourcePath, string savePath, int width, int height, bool useTempFolder = false);

        /// <summary>
        /// Resizes the image by greatest dimension.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="savePath">The save path.</param>
        /// <param name="maxHeightOrWidth">Max height for a portrait image; or, max width for a landscape image.</param>
        void ResizeImageByGreatestDimension(string sourcePath, string savePath, int maxHeightOrWidth);

        /// <summary>
        /// Resizes the image by height and aspect ratio
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="savePath">The save path.</param>
        /// <param name="height">The height.</param>
        void ResizeImageByHeight(string sourcePath, string savePath, int height);

        /// <summary>
        /// Resizes the image by width and aspect ratio.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="savePath">The save path.</param>
        /// <param name="width">The width.</param>
        void ResizeImageByWidth(string sourcePath, string savePath, int width);

        /// <summary>
        /// Rotates the specified source path.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>Rotation info.</returns>
        (Size original, Size rotated) Rotate(string sourcePath, int angle);

        /// <summary>
        /// Gets the image layout information.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>Image layout info.</returns>
        ImageLayoutInfo GetImageLayoutInfo(string sourcePath);

        /// <summary>
        /// Removes the image metadata.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        void RemoveMetadata(string sourcePath);
    }
}