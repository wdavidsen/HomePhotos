using SCS.HomePhotos.Model;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    public class Tag
    {
        public Tag() { }

        public Tag(Model.Tag tag)
        {
            TagId = tag.TagId;
            TagName = tag.TagName;
            PhotoCount = (tag is TagStat) ? ((TagStat)tag).PhotoCount : -1;
        }

        public int? TagId { get; set; }

        [Required]
        public string TagName { get; set; }

        public int PhotoCount { get; set; }

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
