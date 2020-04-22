using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Web.Models
{
    public class BatchUpdateTags
    {
        public BatchUpdateTags()
        {
            TagStates = new List<TagState>();
            PhotoIds = new List<int>();
        }

        public List<TagState> TagStates { get; set; }

        public List<int> PhotoIds { get; set; }

        public List<string> GetAddedTagNames()
        {
            return TagStates.Where(t => t.Checked).Select(o => o.TagName).ToList();
        }

        public List<int> GetRemovedTagIds()
        {
            return TagStates.Where(t => !t.Checked).Select(o => o.Id).ToList();
        }
    }
}
