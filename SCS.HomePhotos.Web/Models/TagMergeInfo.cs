using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Tag merge info model.</summary>
    public class TagMergeInfo
    {
        /// <summary>Creates new tagname.</summary>
        /// <value>The new name of the tag.</value>
        [Required]
        [MaxLength(50)]
        public string NewTagName { get; set; }

        /// <summary>Gets or sets the source tag ids.</summary>
        /// <value>The source tag ids.</value>
        [Required]
        public int[] SourceTagIds { get; set; }
    }
}
