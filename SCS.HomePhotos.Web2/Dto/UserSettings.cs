using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// User specific app settings.
    /// </summary>
    public class UserSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettings"/> class.
        /// </summary>
        public UserSettings()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettings"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public UserSettings(Model.UserSettings settings)
        {
            AutoStartSlideshow = settings.AutoStartSlideshow;
            DefaultView = settings.DefaultView;
            SlideshowSpeed = settings.SlideshowSpeed;
            ThumbnailSize = settings.ThumbnailSize;
            UserScope = settings.UserScope;
        }

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

        /// <summary>
        /// Converts DTO to domain model equivalent.
        /// </summary>
        /// <returns>The equivalent domain entity.</returns>
        public virtual Model.UserSettings ToModel()
        {
            return new Model.UserSettings
            {
                AutoStartSlideshow = AutoStartSlideshow,
                DefaultView = DefaultView,
                SlideshowSpeed = SlideshowSpeed,
                ThumbnailSize = ThumbnailSize,
                UserScope = UserScope
            };
        }
    }
}
