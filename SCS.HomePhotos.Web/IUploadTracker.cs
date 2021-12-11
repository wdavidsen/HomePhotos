namespace SCS.HomePhotos.Web
{
    /// <summary>
    /// Image file upload tracker.
    /// </summary>
    public interface IUploadTracker
    {
        /// <summary>
        /// Increments the add upload counter.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="file">The file.</param>
        void AddUpload(string userName, string file);

        /// <summary>
        /// Clears the upload count.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the upload count.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        int GetUploadCount(string userName);

        /// <summary>
        /// Removes an upload from the counter.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        void RemoveUpload(string file);

        /// <summary>
        /// Determines whether upload processing done for user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>
        ///   <c>true</c> if upload processing done for user; otherwise, <c>false</c>.
        /// </returns>
        bool IsProcessingDone(string userName);
    }
}