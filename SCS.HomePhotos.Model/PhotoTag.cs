using Dapper;

namespace SCS.HomePhotos.Model
{
    [Table("PhotoTag")]
    public class PhotoTag
    {
        [Key]
        public int PhotoTagId { get; set; }
        public int PhotoId { get; set; }
        public int TagId { get; set; }
    }
}
