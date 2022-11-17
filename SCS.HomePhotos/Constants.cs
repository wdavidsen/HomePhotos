namespace SCS.HomePhotos
{
    /// <summary>
    /// Global application constants.
    /// </summary>
    public sealed class Constants
    {
        /// <summary>
        /// The cache route.
        /// </summary>
        public const string CacheRoute = "/photo-image";

        /// <summary>
        /// The avatar route.
        /// </summary>
        public const string AvatarRoute = "/avatar-image";

        /// <summary>
        /// The avatar folder.
        /// </summary>
        public const string AvatarFolder = "_avatars";

        /// <summary>
        /// The avatar default file.
        /// </summary>
        public const string AvatarDefaultFile = "avatar-placeholder.png";

        /// <summary>
        /// The refresh route.
        /// </summary>
        public const string RefreshRoute = "/api/auth/refresh";

        /// <summary>
        /// The database date time format.
        /// </summary>
        public const string DatabaseDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// The default tag color.
        /// </summary>
        public const string DefaultTagColor = "rgb(255, 249, 196)";

        /// <summary>
        /// The accepted extensions.
        /// </summary>
        public readonly static string[] AcceptedExtensions = new string[] { "JPG", "JPEG", "PNG", "GIF", ".JPG", ".JPEG", ".PNG", ".GIF" };
}
}
