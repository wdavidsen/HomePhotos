using Dapper;
using System;
using System.Collections.Generic;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The photo entity.
    /// </summary>
    [Table("Photo")]
    public class Photo
    {
        /// <summary>
        /// Gets or sets the photo identifier.
        /// </summary>
        /// <value>
        /// The photo identifier.
        /// </value>
        [Key]
        public int? PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the image checksum.
        /// </summary>
        /// <value>
        /// The image checksum.
        /// </value>
        public string Checksum { get; set; }

        /// <summary>
        /// Gets or sets the photo name.
        /// </summary>
        /// <value>
        /// The photo name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the image file.
        /// </summary>
        /// <value>
        /// The name of the image ile.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the photo date taken.
        /// </summary>
        /// <value>
        /// The photo date taken.
        /// </value>
        public DateTime? DateTaken { get; set; }

        /// <summary>
        /// Gets or sets the date image file was created.
        /// </summary>
        /// <value>
        /// The date image file was created.
        /// </value>
        public DateTime DateFileCreated { get; set; }

        /// <summary>
        /// Gets or sets the cache folder.
        /// </summary>
        /// <value>
        /// The cache folder.
        /// </value>
        public string CacheFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to reprocess cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if reprocessing cache; otherwise, <c>false</c>.
        /// </value>
        public bool ReprocessCache { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the photo is a mobile upload.
        /// </summary>
        /// <value>
        ///   <c>true</c> if photo is a mobile upload; otherwise, <c>false</c>.
        /// </value>
        public bool MobileUpload { get; set; }

        /// <summary>
        /// Gets or sets the photo's original folder under photo index, or mobile upload path.
        /// </summary>
        /// <value>
        /// The photo's original folder.
        /// </value>
        public string OriginalFolder { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the search weight.
        /// </summary>
        /// <value>
        /// The search weight.
        /// </value>
        [IgnoreInsert]
        [IgnoreUpdate]
        public int Weight { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public IList<Tag> Tags { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is not Photo p) return false;
            if (p.PhotoId != PhotoId) return false;
            if (p.Name != Name) return false;
            if (p.FileName != FileName) return false;
            if (p.DateFileCreated != DateFileCreated) return false;
            if (p.CacheFolder != CacheFolder) return false;
            if (p.Checksum != Checksum) return false;
            if (p.DateTaken != DateTaken) return false;
            if (p.ImageHeight != ImageHeight) return false;
            if (p.ImageWidth != ImageWidth) return false;
            if (p.MobileUpload != MobileUpload) return false;
            if (p.OriginalFolder != OriginalFolder) return false;
            if (p.UserId != UserId) return false;

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return PhotoId.GetHashCode() ^ Name.GetHashCode() ^ FileName.GetHashCode() ^ DateFileCreated.GetHashCode() ^ DateTaken.GetHashCode() ^ CacheFolder.GetHashCode() ^
                Checksum.GetHashCode() ^ ImageHeight ^ ImageWidth ^ MobileUpload.GetHashCode() ^ OriginalFolder.GetHashCode() ^ UserId.GetHashCode();
        }
    }
}
