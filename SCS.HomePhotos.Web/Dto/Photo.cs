using System;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// Photo DTO.
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Photo"/> class.
        /// </summary>
        public Photo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Photo"/> class using the domain model class.
        /// </summary>
        /// <param name="photo">The photo.</param>
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

        /// <summary>
        /// Gets or sets the photo identifier.
        /// </summary>
        /// <value>
        /// The photo identifier.
        /// </value>
        public int? PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        [Required]
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        [Required]
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the date taken.
        /// </summary>
        /// <value>
        /// The date taken.
        /// </value>
        [Required]
        public DateTime? DateTaken { get; set; }

        /// <summary>
        /// Gets or sets the date file created.
        /// </summary>
        /// <value>
        /// The date file created.
        /// </value>
        [Required]
        public DateTime DateFileCreated { get; set; }

        /// <summary>
        /// Gets or sets the cache folder.
        /// </summary>
        /// <value>
        /// The cache folder.
        /// </value>
        [Required]
        public string CacheFolder { get; set; }

        /// <summary>
        /// Converts instance to the domain model.
        /// </summary>
        /// <returns>The domain equivalent instance.</returns>
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
