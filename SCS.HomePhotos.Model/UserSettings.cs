using Dapper;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// Photo thumbnail size.
    /// </summary>
    public enum ThumbnailSize
    {
        /// <summary>
        /// The smallest size.
        /// </summary>
        Smallest = 1,
        /// <summary>
        /// The small size.
        /// </summary>
        Small = 2,
        /// <summary>
        /// The medium size.
        /// </summary>
        Medium = 3,
        /// <summary>
        /// The large size.
        /// </summary>
        Large = 4,
        /// <summary>
        /// The largest size.
        /// </summary>
        Largest = 5
    }

    /// <summary>
    /// Photo slideshot speed.
    /// </summary>
    public enum SlideshowSpeed
    {
        /// <summary>
        /// The slowest speed.
        /// </summary>
        Slowest = 1,
        /// <summary>
        /// The slow speed.
        /// </summary>
        Slow = 2,
        /// <summary>
        /// The normal speed.
        /// </summary>
        Normal = 3,
        /// <summary>
        /// The fast speed.
        /// </summary>
        Fast = 4,
        /// <summary>
        /// The fastest speed.
        /// </summary>
        Fastest = 5
    }

    /// <summary>
    /// User settings
    /// </summary>
    [Table("UserSettings")]
    public class UserSettings
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the size of the thumbnail.
        /// </summary>
        /// <value>
        /// The size of the thumbnail.
        /// </value>
        public ThumbnailSize ThumbnailSize { get; set; }

        /// <summary>
        /// Gets or sets the slideshow speed.
        /// </summary>
        /// <value>
        /// The slideshow speed.
        /// </value>
        public SlideshowSpeed SlideshowSpeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start slideshow should start automatically.
        /// </summary>
        /// <value>
        ///   <c>true</c> if start slideshow should start automatically; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStartSlideshow { get; set; }

        /// <summary>
        /// Gets or sets the default view.
        /// </summary>
        /// <value>
        /// The default view.
        /// </value>
        public string DefaultView { get; set; }

        /// <summary>
        /// Gets or sets the user scope.
        /// </summary>
        /// <value>
        /// The user scope.
        /// </value>
        public UserPhotoScope UserScope { get; set; }
    }
}
