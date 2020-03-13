namespace SCS.HomePhotos.Web.Models
{
    /// <summary>
    /// Represents a file upload error.
    /// </summary>
    public class FileError
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }
    }
}
