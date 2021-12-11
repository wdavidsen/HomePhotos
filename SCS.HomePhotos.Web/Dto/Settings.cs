using System;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// Settings DTO.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class using a <see cref="IDynamicConfig"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public Settings(IDynamicConfig config)
        {
            ConfigId = config.ConfigId;
            CacheFolder = config.CacheFolder;
            MobileUploadsFolder = config.MobileUploadsFolder;
            IndexPath = config.IndexPath;
            LargeImageSize = config.LargeImageSize;
            SmallImageSize = config.SmallImageSize;
            NextIndexTime = config.NextIndexTime;
            IndexFrequencyHours = config.IndexFrequencyHours;
            ThumbnailSize = config.ThumbnailSize;
        }

        /// <summary>
        /// Gets or sets the configuration identifier.
        /// </summary>
        /// <value>
        /// The configuration identifier.
        /// </value>
        public int? ConfigId { get; set; }

        /// <summary>
        /// Gets or sets the cache folder.
        /// </summary>
        /// <value>
        /// The cache folder.
        /// </value>
        [Required]
        public string CacheFolder { get; set; }

        /// <summary>
        /// Gets or sets the mobile uploads folder.
        /// </summary>
        /// <value>
        /// The mobile uploads folder.
        /// </value>
        [Required]
        public string MobileUploadsFolder { get; set; }

        /// <summary>
        /// Gets or sets the index path.
        /// </summary>
        /// <value>
        /// The index path.
        /// </value>
        [Required]
        public string IndexPath { get; set; }

        /// <summary>
        /// Gets or sets the size of the large image.
        /// </summary>
        /// <value>
        /// The size of the large image.
        /// </value>
        [Required]
        public int LargeImageSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the small image.
        /// </summary>
        /// <value>
        /// The size of the small image.
        /// </value>
        [Required]
        public int SmallImageSize { get; set; }

        /// <summary>
        /// Gets or sets the next index time.
        /// </summary>
        /// <value>
        /// The next index time.
        /// </value>
        [Required]
        public DateTime? NextIndexTime { get; set; }

        /// <summary>
        /// Gets or sets the index frequency hours.
        /// </summary>
        /// <value>
        /// The index frequency hours.
        /// </value>
        [Required]
        public int IndexFrequencyHours { get; set; }

        /// <summary>
        /// Gets or sets the size of the thumbnail.
        /// </summary>
        /// <value>
        /// The size of the thumbnail.
        /// </value>
        [Required]
        public int ThumbnailSize { get; set; }

        /// <summary>
        /// Converts instance to the domain model.
        /// </summary>
        /// <returns>The domain equivalent instance.</returns>
        public Model.Config ToModel()
        {
            return new Model.Config
            {
                ConfigId = ConfigId,
                CacheFolder = CacheFolder,
                MobileUploadsFolder = MobileUploadsFolder,
                IndexPath = IndexPath,
                LargeImageSize = LargeImageSize,
                SmallImageSize = SmallImageSize,
                NextIndexTime = NextIndexTime,
                IndexFrequencyHours = IndexFrequencyHours,
                ThumbnailSize = ThumbnailSize
            };
        }

        /// <summary>
        /// Converts to dynamic config.
        /// </summary>
        /// <returns>A <see cref="IDynamicConfig"/> object.</returns>
        public DynamicConfig ToDynamicConfig()
        {
            return new DynamicConfig
            {
                ConfigId = ConfigId,
                CacheFolder = CacheFolder,
                IndexPath = IndexPath,
                LargeImageSize = LargeImageSize,
                SmallImageSize = SmallImageSize,
                NextIndexTime = NextIndexTime,
                IndexFrequencyHours = IndexFrequencyHours,
                ThumbnailSize = ThumbnailSize
            };
        }
    }
}
