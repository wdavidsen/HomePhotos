namespace SCS.HomePhotos.Web.Models
{
    /// <summary>
    /// Wrapper for uploading multiple files.
    /// </summary>
    /// <typeparam name="T">The file group type.</typeparam>
    public class FileGroup<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileGroup{T}"/> class.
        /// </summary>
        public FileGroup()
        {
            Files = new List<T>();
        }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<T> Files { get; }
    }
}
