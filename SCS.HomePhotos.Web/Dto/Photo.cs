using System;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    public class Photo
    {
        public Photo() { }

        public Photo(Model.Photo photo)
        {
            PhotoId = photo.PhotoId;
            Name = photo.Name;
            FileName = photo.FileName;
            DateTaken = photo.DateTaken;
            DateFileCreated = photo.DateFileCreated;
            CacheFolder = photo.CacheFolder;
            ImageHeight = photo.ImageHeight;
            ImageWidth = photo.ImageWidth;
        }

        public int? PhotoId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public int ImageHeight { get; set; }

        [Required]
        public int ImageWidth { get; set; }

        [Required]
        public DateTime? DateTaken { get; set; }

        [Required]
        public DateTime DateFileCreated { get; set; }

        [Required]
        public string CacheFolder { get; set; }

        public Model.Photo ToModel()
        {
            var photo = new Model.Photo
            {
                PhotoId = PhotoId,
                Name = Name,
                FileName = FileName,
                DateTaken = DateTaken,
                DateFileCreated = DateFileCreated,
                CacheFolder = CacheFolder,
                ImageHeight = ImageHeight,
                ImageWidth = ImageWidth
            };

            return photo;
        }
    }
}
