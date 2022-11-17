using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Tag merge info model.</summary>
    public class TagMergeInfo
    {
        /// <summary>Creates new tag name.</summary>
        /// <value>The new name of the tag.</value>
        [Required]
        [MaxLength(50)]
        public string NewTagName { get; set; }

        /// <summary>Gets or sets the source tag ids.</summary>
        /// <value>The source tag ids.</value>
        [Required]
        public int[] SourceTagIds { get; set; }

        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>
        /// The owner identifier.
        /// </value>
        public int? OwnerId { get; set; }
    }
}
