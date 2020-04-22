using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Web.Models
{
    public class BatchSelectTags
    {
        public BatchSelectTags(IEnumerable<int> photoIds, IEnumerable<Tag> tags)
        {
            if (photoIds == null)
            {
                throw new ArgumentNullException(nameof(photoIds));
            }
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }
            if (photoIds.Count() < 1)
            {
                throw new ArgumentException("At lease one photo must be selected.", nameof(photoIds));
            }

            Tags = new List<AmbiguousTagState>();

            foreach (var tag in tags)
            {
                var tagState = new AmbiguousTagState
                {
                    Id = tag.TagId.Value,
                    TagName = tag.TagName
                };

                var matchCount = tag.Photos.Where(p => photoIds.Contains(p.PhotoId.Value)).Count();

                tagState.Checked = (matchCount == photoIds.Count());
                tagState.AllowIndeterminate = tagState.Indeterminate = (matchCount != photoIds.Count());

                Tags.Add(tagState);
            }
        }

        public List<AmbiguousTagState> Tags { get; set; }
    }
}
