using System;

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
        }

        public int? PhotoId { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public DateTime? DateTaken { get; set; }

        public DateTime DateFileCreated { get; set; }

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
                CacheFolder = CacheFolder
            };

            return photo;
        }
    }
}
