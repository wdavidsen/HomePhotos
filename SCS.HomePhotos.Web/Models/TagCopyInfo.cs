using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    public class TagCopyInfo
    {
        [Required]
        public int? SourceTagId { get; set; }

        [Required]
        [MaxLength(50)]
        public string NewTagName { get; set; }
    }
}
