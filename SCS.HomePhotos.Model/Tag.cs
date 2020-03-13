using Dapper;

namespace SCS.HomePhotos.Model
{
    [Table("Tag")]
    public class Tag
    {
        [Key]
        public int? TagId { get; set; }
        public string TagName { get; set; }
    }
}
