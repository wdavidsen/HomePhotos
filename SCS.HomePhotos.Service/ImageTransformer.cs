using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;

namespace SCS.HomePhotos.Service
{
    /// <summary>
    /// Helper class for common image operations.
    /// </summary>
    public class ImageTransformer : IImageTransformer
    {
        private readonly ILogger<ImageTransformer> _logger;
        private readonly IStaticConfig _staticConfig;

        public ImageTransformer(ILogger<ImageTransformer> logger, IStaticConfig staticConfig)
        {
            _logger = logger;
            _staticConfig = staticConfig;
        }

        /// <summary>
        /// Resizes an image from disk proportionately by its width.
        /// </summary>
        /// <param name="sourcePath">The source image file path.</param>
        /// <param name="savePath">The save file path.</param>
        /// <param name="width">The width.</param>
        public void ResizeImageByWidth(string sourcePath, string savePath, int width)
        {
            ResizeImage(sourcePath, savePath, width, -1);
        }

        /// <summary>
        /// Resizes an image from disk proportionately by its height.
        /// </summary>
        /// <param name="sourcePath">The source image file path.</param>
        /// <param name="savePath">The save file path.</param>
        /// <param name="height">The height.</param>
        public void ResizeImageByHeight(string sourcePath, string savePath, int height)
        {
            ResizeImage(sourcePath, savePath, -1, height);
        }

        /// <summary>
        /// Resizes an image from disk proportionately by its height or width whether is a portrait or landscaped image respectively.
        /// </summary>
        /// <param name="sourcePath">The source image file path.</param>
        /// <param name="savePath">The save file path.</param>
        /// <param name="maxHeightOrWidth">Max height for a portrait image; or, max width for a landscape image.</param>
        public void ResizeImageByGreatestDimension(string sourcePath, string savePath, int maxHeightOrWidth)
        {
            var imageInfo = GetImageLayoutInfo(sourcePath);

            if (imageInfo.LayoutType == ImageLayoutType.Landscape)
            {
                ResizeImageByWidth(sourcePath, savePath, maxHeightOrWidth);
            }
            else
            {
                ResizeImageByHeight(sourcePath, savePath, maxHeightOrWidth);
            }
        }

        /// <summary>
        /// Resizes an image from disk.
        /// </summary>
        /// <param name="sourcePath">The source image file path.</param>
        /// <param name="savePath">The save file path.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void ResizeImage(string sourcePath, string savePath, int width, int height, bool useTempFolder = false)
        {
            var tempPath = "";
            var tempPathResized = "";
              
            try
            {
                if (useTempFolder)
                {
                    tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(sourcePath));
                    tempPathResized = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(sourcePath) + "_thumb" + Path.GetExtension(sourcePath));

                    File.Copy(sourcePath, tempPath, true);
                }

                // https://github.com/SixLabors/ImageSharp
                // Image.Load(string path) is a shortcut for our default type.
                // Other pixel formats use Image.Load<TPixel>(string path))
                using (var image = Image.Load(useTempFolder ? tempPath : sourcePath))
                {
                    if (height < 1)
                    {
                        decimal ratio = image.Height / (decimal)image.Width;
                        height = (int)decimal.Round(width * ratio);
                    }
                    else if (width < 1)
                    {
                        decimal ratio = image.Width / (decimal)image.Height;
                        width = (int)decimal.Round(height * ratio);
                    }

                    var encoder = new JpegEncoder()
                    {
                        Quality = _staticConfig.ImageResizeQuality,
                        Subsample = JpegSubsample.Ratio444
                    };

                    image.Mutate(x => x.Resize(width, height));
                    image.Save(useTempFolder ? tempPathResized : savePath, encoder);

                    if (useTempFolder)
                    {
                        File.Move(tempPathResized, savePath, true);
                    }
                }
            }
            catch (OutOfMemoryException ex)
            {
                GC.Collect(2);
                GC.WaitForPendingFinalizers();

                _logger.LogError(ex, "Ran out of memory while resizing file {sourcePath}.", sourcePath);

                throw;
            }
            finally
            {
                if (useTempFolder)
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                    if (File.Exists(tempPathResized))
                    {
                        File.Delete(tempPathResized);
                    }
                }
            }
        }

        /// <summary>
        /// Rotates an image x degrees.
        /// </summary>
        /// <param name="sourcePath">The image to rotate.</param>
        /// <param name="angle">The rotation angle.</param>        
        /// <returns>The original and rotated sizes.</returns>
        public (Size original, Size rotated) Rotate(string sourcePath, int angle)
        {
            using (var image = Image.Load<Rgba32>(sourcePath))
            {
                var original = image.Size();
                image.Mutate(x => x.Rotate(angle));
                image.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);

                image.Save(sourcePath);
                
                return (original, image.Size());
            }
        }

        /// <summary>
        /// Gets basic image information.
        /// </summary>
        /// <param name="sourcePath">The source image file path.</param>
        /// <returns>Image information.</returns>
        public ImageLayoutInfo GetImageLayoutInfo(string sourcePath)
        {
            var info = ImageLayoutInfo.Default;

            using (var image = Image.Load(sourcePath))
            {
                info.Height = image.Height;
                info.Width = image.Width;

                decimal ratio = image.Width / (decimal)image.Height;
                info.Ratio = ratio;

                if (ratio > 1)
                {
                    info.LayoutType = ImageLayoutType.Landscape;
                }
                else
                {
                    info.LayoutType = ImageLayoutType.Portrait;
                }
            }
            return info;
        }


        /// <summary>
        /// Deletes the file at provided path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        protected void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Removes image metadata from image.
        /// </summary>
        /// <param name="sourcePath">The image to rotate.</param>
        public void RemoveMetadata(string sourcePath)
        {
            using (var image = Image.Load<Rgba32>(sourcePath))
            {
                image.Metadata.ExifProfile = null;
                image.Save(sourcePath);
            }
        }
    }
}
