using System.Collections.Generic;

namespace SCS.HomePhotos.Service.Workers
{
    /// <summary>
    /// A list of index paths.
    /// </summary>    
    internal class IndexPaths : List<IndexPath>
    {
    }

    /// <summary>
    /// Combines an index path with a flag indicating whether it is a mobile upload folder.
    /// </summary>
    internal class IndexPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPath"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="mobileUploadFolder">if set to <c>true</c> [mobile upload folder].</param>
        public IndexPath(string path, bool mobileUploadFolder)
        {
            DirectoryPath = path;
            IsMobileUpload = mobileUploadFolder;    
        }

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        /// <value>
        /// The directory path.
        /// </value>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mobile upload.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mobile upload; otherwise, <c>false</c>.
        /// </value>
        public bool IsMobileUpload { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{DirectoryPath}; Mobile={IsMobileUpload}";
        }
    }
}
