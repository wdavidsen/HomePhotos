using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Batch update tag info.</summary>
    public class BatchUpdateTags
    {
        /// <summary>Initializes a new instance of the <see cref="BatchUpdateTags" /> class.</summary>
        public BatchUpdateTags()
        {
            TagStates = new List<TagState>();
            PhotoIds = new List<int>();
        }

        /// <summary>Gets or sets the tag states.</summary>
        /// <value>The tag states.</value>
        public List<TagState> TagStates { get; set; }

        /// <summary>Gets or sets the photo ids.</summary>
        /// <value>The photo ids.</value>
        public List<int> PhotoIds { get; set; }

        /// <summary>Gets the added tag names.</summary>
        /// <returns>A list of tag names.</returns>
        public List<string> GetAddedTagNames()
        {
            return TagStates.Where(t => t.Checked).Select(o => o.TagName).ToList();
        }

        /// <summary>Gets the removed tag ids.</summary>
        /// <returns>Tag ids to remove.</returns>
        public List<int> GetRemovedTagIds()
        {
            return TagStates.Where(t => !t.Checked).Select(o => o.Id).ToList();
        }
    }
}
