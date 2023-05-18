using System;

namespace SCS.HomePhotos
{
    /// <summary>
    /// Photo user scope.
    /// </summary>
    public enum UserPhotoScope
    {
        /// <summary>
        /// All photos.
        /// </summary>
        Everything = 0,
        /// <summary>
        /// Shared and personal photos.
        /// </summary>
        SharedAndPersonal = 10,
        /// <summary>
        /// Personal photos only.
        /// </summary>
        PersonalOnly = 20
    }
}
