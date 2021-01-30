namespace SCS.HomePhotos
{
    public class Constants
    {
        public const string CacheRoute = "/photo-image";
        public const string AvatarRoute = "/avatar-image";
        public const string AvatarFolder = "_avatars";
        public const string AvatarDefaultFile = "avatar-placeholder.png";
        public const string RefreshRoute = "/api/auth/refresh";

        public readonly static string[] AcceptedExtensions = new string[] { "JPG", "JPEG", "PNG", "GIF", ".JPG", ".JPEG", ".PNG", ".GIF" };
}
}
