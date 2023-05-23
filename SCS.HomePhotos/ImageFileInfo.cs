using System;

namespace SCS.HomePhotos
{
    /// <summary>
    /// The source of the image file.
    /// </summary>
    public enum ImageFileSource
    {
        /// <summary>
        /// The image is from local disk.
        /// </summary>
        LocalDisk = 1,

        /// <summary>
        /// The image is a mobile upload.
        /// </summary>
        MobileUpload = 2
    }

    /// <summary>
    /// Image file info.
    /// </summary>
    public struct ImageFileInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileInfo"/> struct.
        /// </summary>
        /// <param name="imageFileSource">The image file source.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="cacheFilePath">The cache file path.</param>
        /// <param name="checksum">The checksum.</param>
        public ImageFileInfo(ImageFileSource imageFileSource, string imageFilePath, string cacheFilePath, string checksum)
        {
            ImageFileSource = imageFileSource;
            ImageFilePath = imageFilePath;
            CacheFilePath = cacheFilePath;
            Checksum = checksum;
        }

        /// <summary>
        /// Gets or sets the image file source.
        /// </summary>
        /// <value>
        /// The image file source.
        /// </value>
        public ImageFileSource ImageFileSource { get; set; }

        /// <summary>
        /// Gets or sets the image file path.
        /// </summary>
        /// <value>
        /// The image file path.
        /// </value>
        public string ImageFilePath { get; set; }

        /// <summary>
        /// Gets or sets the cache file path.
        /// </summary>
        /// <value>
        /// The cache file path.
        /// </value>
        public string CacheFilePath { get; set; }

        /// <summary>
        /// Gets or sets the checksum.
        /// </summary>
        /// <value>
        /// The checksum.
        /// </value>
        public string Checksum { get; set; }
    }
}
