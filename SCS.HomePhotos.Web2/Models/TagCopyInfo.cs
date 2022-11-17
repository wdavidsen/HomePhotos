using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Tag copy info model.</summary>
    public class TagCopyInfo
    {
        /// <summary>Gets or sets the source tag identifier.</summary>
        /// <value>The source tag identifier.</value>
        [Required]
        public int? SourceTagId { get; set; }

        /// <summary>Creates new tag name.</summary>
        /// <value>The new name of the tag.</value>
        [Required]
        [MaxLength(50)]
        public string NewTagName { get; set; }

        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>
        /// The owner identifier.
        /// </value>
        public int? OwnerId { get; set; }
    }
}
