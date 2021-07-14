using Dapper;
using System.Collections.Generic;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The tag entity.
    /// </summary>
    [Table("Tag")]
    public class Tag
    {
        /// <summary>
        /// Gets or sets the tag identifier.
        /// </summary>
        /// <value>
        /// The tag identifier.
        /// </value>
        [Key]
        public int? TagId { get; set; }

        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        /// <value>
        /// The name of the tag.
        /// </value>
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the photos.
        /// </summary>
        /// <value>
        /// The photos.
        /// </value>
        public IList<Photo> Photos { get; set; }
    }
}
