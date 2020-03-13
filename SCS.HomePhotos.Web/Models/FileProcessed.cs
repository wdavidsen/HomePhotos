namespace SCS.HomePhotos.Web.Models
{
    /// <summary>
    /// Processed file information.
    /// </summary>
    public class FileProcessed
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        /// <value>
        /// The original name.
        /// </value>
        public string OriginalName { get; set; }

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the image thumbnail URL.
        /// </summary>
        /// <value>
        /// The thumbnail URL.
        /// </value>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the delete URL for uploaded file.
        /// </summary>
        /// <value>
        /// The delete URL.
        /// </value>
        public string DeleteUrl { get; set; }

        /// <summary>
        /// Gets or sets the type of the delete.
        /// </summary>
        /// <value>
        /// The type of the delete.
        /// </value>
        public string DeleteType { get; set; }
    }
}
