using SCS.HomePhotos.Model;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// Tab DTO.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        public Tag() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class using the domain model class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public Tag(Model.Tag tag)
        {
            TagId = tag.TagId;
            TagName = tag.TagName;
            PhotoCount = (tag is TagStat) ? ((TagStat)tag).PhotoCount : -1;
        }

        /// <summary>
        /// Gets or sets the tag identifier.
        /// </summary>
        /// <value>
        /// The tag identifier.
        /// </value>
        public int? TagId { get; set; }

        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        /// <value>
        /// The name of the tag.
        /// </value>
        [Required]
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the photo count.
        /// </summary>
        /// <value>
        /// The photo count.
        /// </value>
        public int PhotoCount { get; set; }

        /// <summary>
        /// Converts instance to the domain model.
        /// </summary>
        /// <returns>The domain equivalent instance.</returns>
        public Model.Tag ToModel()
        {
            var tag = new Model.TagStat
            {
                TagId = TagId,
                TagName = TagName,
                PhotoCount = PhotoCount
            };

            return tag;
        }
    }
}
