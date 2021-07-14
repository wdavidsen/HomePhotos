using Dapper;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The photo-tag entity.
    /// </summary>
    [Table("PhotoTag")]
    public class PhotoTag
    {
        /// <summary>
        /// Gets or sets the photo tag identifier.
        /// </summary>
        /// <value>
        /// The photo tag identifier.
        /// </value>
        [Key]
        public int? PhotoTagId { get; set; }

        /// <summary>
        /// Gets or sets the photo identifier.
        /// </summary>
        /// <value>
        /// The photo identifier.
        /// </value>
        public int PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the tag identifier.
        /// </summary>
        /// <value>
        /// The tag identifier.
        /// </value>
        public int TagId { get; set; }
    }
}
