using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Batch select tags info.</summary>
    public class BatchSelectTags
    {
        /// <summary>Initializes a new instance of the <see cref="BatchSelectTags" /> class.</summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <param name="tags">The tags.</param>
        /// <exception cref="ArgumentNullException">photoIds
        /// or
        /// tags</exception>
        /// <exception cref="ArgumentException">At lease one photo must be selected. - photoIds</exception>
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
            if (!photoIds.Any())
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

        /// <summary>Gets or sets the tags.</summary>
        /// <value>The tags.</value>
        public List<AmbiguousTagState> Tags { get; set; }
    }
}
