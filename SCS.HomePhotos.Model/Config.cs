using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The configuration entity.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.IConfig" />
    [Table("Config")]
    public class Config : IConfig
    {
        /// <summary>
        /// Gets or sets the configuration identifier.
        /// </summary>
        /// <value>
        /// The configuration identifier.
        /// </value>
        [Key]
        public int? ConfigId { get; set; }

        /// <summary>
        /// Gets or sets the index path.
        /// </summary>
        /// <value>
        /// The index path.
        /// </value>
        public string IndexPath { get; set; }

        /// <summary>
        /// Gets or sets the cache folder.
        /// </summary>
        /// <value>
        /// The cache folder.
        /// </value>
        public string CacheFolder { get; set; }

        /// <summary>
        /// Gets or sets the mobile uploads folder.
        /// </summary>
        /// <value>
        /// The mobile uploads folder.
        /// </value>
        public string MobileUploadsFolder { get; set; }

        /// <summary>
        /// Gets or sets the size of the thumbnail.
        /// </summary>
        /// <value>
        /// The size of the thumbnail.
        /// </value>
        public int ThumbnailSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the small image.
        /// </summary>
        /// <value>
        /// The size of the small image.
        /// </value>
        public int SmallImageSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the large image.
        /// </summary>
        /// <value>
        /// The size of the large image.
        /// </value>
        public int LargeImageSize { get; set; }

        /// <summary>
        /// Gets or sets the index frequency hours.
        /// </summary>
        /// <value>
        /// The index frequency hours.
        /// </value>
        public int IndexFrequencyHours { get; set; }

        /// <summary>
        /// Gets or sets the next index time.
        /// </summary>
        /// <value>
        /// The next index time.
        /// </value>
        public DateTime? NextIndexTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to index on startup.
        /// </summary>
        /// <value>
        /// <c>true</c> if indexing on startup; otherwise, <c>false</c>.
        /// </value>
        public bool IndexOnStartup { get; set; }

        /// <summary>
        /// Converts configuration to dynamic configuration entity.
        /// </summary>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        public void ToDynamicConfig(IDynamicConfig dynamicConfig)
        {
            dynamicConfig.ConfigId = ConfigId;
            dynamicConfig.CacheFolder = CacheFolder;
            dynamicConfig.MobileUploadsFolder = MobileUploadsFolder;
            dynamicConfig.IndexPath = IndexPath;
            dynamicConfig.ThumbnailSize = ThumbnailSize;
            dynamicConfig.SmallImageSize = SmallImageSize;
            dynamicConfig.LargeImageSize = LargeImageSize;
            dynamicConfig.NextIndexTime = NextIndexTime;
            dynamicConfig.IndexFrequencyHours = IndexFrequencyHours;
            dynamicConfig.IndexOnStartup = IndexOnStartup;
        }

        /// <summary>
        /// Updates configuration from a dynamic configuration.
        /// </summary>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        public void FromDynamicConfig(IDynamicConfig dynamicConfig)
        {
            ConfigId = dynamicConfig.ConfigId;
            CacheFolder = dynamicConfig.CacheFolder;
            MobileUploadsFolder = dynamicConfig.MobileUploadsFolder;
            IndexPath = dynamicConfig.IndexPath;
            ThumbnailSize = dynamicConfig.ThumbnailSize;
            SmallImageSize = dynamicConfig.SmallImageSize;
            LargeImageSize = dynamicConfig.LargeImageSize;
            NextIndexTime = dynamicConfig.NextIndexTime;
            IndexFrequencyHours = dynamicConfig.IndexFrequencyHours;
            IndexOnStartup = dynamicConfig.IndexOnStartup;
        }
    }
}
