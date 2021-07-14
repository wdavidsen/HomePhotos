using Dapper;
using System;
using System.Collections.Generic;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The photo entity.
    /// </summary>
    [Table("Photo")]
    public class Photo
    {
        /// <summary>
        /// Gets or sets the photo identifier.
        /// </summary>
        /// <value>
        /// The photo identifier.
        /// </value>
        [Key]
        public int? PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the image checksum.
        /// </summary>
        /// <value>
        /// The image checksum.
        /// </value>
        public string Checksum { get; set; }

        /// <summary>
        /// Gets or sets the photo name.
        /// </summary>
        /// <value>
        /// The photo name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the image file.
        /// </summary>
        /// <value>
        /// The name of the image ile.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the photo date taken.
        /// </summary>
        /// <value>
        /// The photo date taken.
        /// </value>
        public DateTime? DateTaken { get; set; }

        /// <summary>
        /// Gets or sets the date image file was created.
        /// </summary>
        /// <value>
        /// The date image file was created.
        /// </value>
        public DateTime DateFileCreated { get; set; }

        /// <summary>
        /// Gets or sets the cache folder.
        /// </summary>
        /// <value>
        /// The cache folder.
        /// </value>
        public string CacheFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to reprocess cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if reprocessing cache; otherwise, <c>false</c>.
        /// </value>
        public bool ReprocessCache { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public IList<Tag> Tags { get; set; }
    }
}
