using System;

namespace SCS.HomePhotos
{
    /// <summary>
    /// The application configuration.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets the cache folder.
        /// </summary>
        /// <value>
        /// The cache folder.
        /// </value>
        string CacheFolder { get; set; }

        /// <summary>
        /// Gets or sets the mobile uploads folder.
        /// </summary>
        /// <value>
        /// The mobile uploads folder.
        /// </value>
        string MobileUploadsFolder { get; set; }

        /// <summary>
        /// Gets or sets the configuration identifier.
        /// </summary>
        /// <value>
        /// The configuration identifier.
        /// </value>
        int? ConfigId { get; set; }

        /// <summary>
        /// Gets or sets the index path.
        /// </summary>
        /// <value>
        /// The index path.
        /// </value>
        string IndexPath { get; set; }

        /// <summary>
        /// Gets or sets the size of the large image.
        /// </summary>
        /// <value>
        /// The size of the large image.
        /// </value>
        int LargeImageSize { get; set; }

        /// <summary>
        /// Gets or sets the next index time.
        /// </summary>
        /// <value>
        /// The next index time.
        /// </value>
        DateTime? NextIndexTime { get; set; }

        /// <summary>
        /// Gets or sets the index frequency hours.
        /// </summary>
        /// <value>
        /// The index frequency hours.
        /// </value>
        int IndexFrequencyHours { get; set; }

        /// <summary>
        /// Gets or sets the size of the thumbnail.
        /// </summary>
        /// <value>
        /// The size of the thumbnail.
        /// </value>
        int ThumbnailSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the small image.
        /// </summary>
        /// <value>
        /// The size of the small image.
        /// </value>
        int SmallImageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to index on startup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if indexing on startup; otherwise, <c>false</c>.
        /// </value>
        bool IndexOnStartup { get; set; }
    }
}