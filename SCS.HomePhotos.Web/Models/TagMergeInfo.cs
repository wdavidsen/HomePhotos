using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    public class TagMergeInfo
    {
        [Required]
        [MaxLength(50)]
        public string NewTagName { get; set; }

        [Required]
        public int[] SourceTagIds { get; set; }
    }
}
