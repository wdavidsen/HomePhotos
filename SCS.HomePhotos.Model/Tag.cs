using Dapper;
using System.Collections.Generic;

namespace SCS.HomePhotos.Model
{
    [Table("Tag")]
    public class Tag
    {
        [Key]
        public int? TagId { get; set; }
        public string TagName { get; set; }

        public IList<Photo> Photos { get; set; }
    }
}
