namespace SCS.HomePhotos
{
    /// <summary>
    /// The delete action to take when deleting a photo.
    /// </summary>
    public enum DeleteAction
    {
        /// <summary>
        /// Delete the database record only.
        /// </summary>
        DeleteRecord = 0,

        /// <summary>
        /// Delete the database record and the image file.
        /// </summary>
        DeleteRecordAndFile = 1
    }
}
