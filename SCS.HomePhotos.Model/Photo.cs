using Dapper;
using System;
using System.Collections.Generic;

namespace SCS.HomePhotos.Model
{
    [Table("Photo")]
    public class Photo
    {
        [Key]
        public int? PhotoId { get; set; }

        public string Checksum { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public int ImageHeight { get; set; }

        public int ImageWidth { get; set; }

        public DateTime? DateTaken { get; set; }

        public DateTime DateFileCreated { get; set; }

        public string CacheFolder { get; set; }

        public IList<Tag> Tags { get; set; }
    }
}
