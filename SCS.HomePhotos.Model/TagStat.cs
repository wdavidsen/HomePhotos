﻿namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The tag-stat entity.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Model.Tag" />
    public sealed class TagStat : Tag
    {
        /// <summary>
        /// Gets or sets the photo count.
        /// </summary>
        /// <value>
        /// The photo count.
        /// </value>
        public int PhotoCount { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public int Weight { get; set; }
    }
}
