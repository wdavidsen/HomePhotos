using Dapper;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// Contains photo, tag, and user info.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Model.PhotoTag" />
    public class UserPhotoTag : PhotoTag
    {
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        /// <value>
        /// The name of the tag.
        /// </value>
        [IgnoreInsert]
        [IgnoreUpdate]
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [IgnoreInsert]
        [IgnoreUpdate]
        public int? UserId { get; set; }
    }
}
