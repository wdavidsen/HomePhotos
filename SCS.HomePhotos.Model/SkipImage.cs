﻿using Dapper;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The skip image entity.
    /// </summary>
    [Table("SkipImage")]
    public class SkipImage
    {
        /// <summary>
        /// Gets or sets the skip image identifier.
        /// </summary>
        /// <value>
        /// The skip image identifier.
        /// </value>
        [Key]
        public int? SkipImageId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether image is a mobile upload.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is a mobile upload; otherwise, <c>false</c>.
        /// </value>
        public bool MobileUpload{ get; set; }

        /// <summary>
        /// Gets or sets the original folder.
        /// </summary>
        /// <value>
        /// The original folder.
        /// </value>
        public string OriginalFolder { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }
    }
}
