using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Web.Models
{
    public class BatchUpdateTags
    {
        public BatchUpdateTags()
        {
            Tags = new List<TagState>();
            PhotoIds = new List<int>();
        }

        public List<TagState> Tags { get; set; }
        public List<int> PhotoIds { get; set; }

        public List<int> GetAddedTagIds()
        {
            return Tags.Where(t => t.Checked).Select(o => o.Id).ToList();
        }

        public List<int> GetRemovedTagIds()
        {
            return Tags.Where(t => !t.Checked).Select(o => o.Id).ToList();
        }
    }
}
