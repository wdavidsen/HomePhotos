using System;
using System.Collections.Generic;

namespace SCS.HomePhotos
{
    /// <summary>
    /// A wrapper for pertinant image information.
    /// </summary>
    public class ImageInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class.
        /// </summary>
        public ImageInfo()
        {
            Tags = new List<string>();
        }

        /// <summary>
        /// Gets or sets the date image was taken.
        /// </summary>
        /// <value>
        /// The date taken.
        /// </value>
        public DateTime DateTaken { get; set; }

        /// <summary>
        /// Gets or sets the image's tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public List<string> Tags { get; set; }
    }
}
