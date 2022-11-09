using SCS.HomePhotos.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The tag repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IDataBase" />
    public interface ITagData : IDataBase
    {
        /// <summary>
        /// Gets all tags.
        /// </summary>
        /// <returns>A list of all tags.</returns>
        Task<IEnumerable<Tag>> GetTags();

        /// <summary>
        /// Gets the tags by keyword.
        /// </summary>
        /// <param name="keywords">The search keywords.</param>
        /// <param name="dateRange">The optional date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of matching tags.</returns>
        Task<IEnumerable<TagStat>> GetTags(string keywords, DateRange? dateRange = null, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets the tags by keyword.
        /// </summary>
        /// <param name="dateRange">The date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of matching tags.</returns>
        Task<IEnumerable<TagStat>> GetTags(DateRange dateRange, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets the tag and photo count.
        /// </summary>
        /// <returns>A list of tags.</returns>
        Task<IEnumerable<TagStat>> GetTagAndPhotoCount();

        /// <summary>
        /// Gets the tag and photo count.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>The tag and photo count.</returns>
        Task<TagStat> GetTagAndPhotoCount(string tagName);

        /// <summary>
        /// Saves a tag.
        /// </summary>
        /// <param name="tag">The tag to save.</param>
        /// <returns>The saved tag.</returns>
        Task<Tag> SaveTag(Tag tag);

        /// <summary>
        /// Gets the photos-tag associations for a tag id.
        /// </summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo-tags.</returns>
        Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int tagId);

        /// <summary>
        /// Gets the photos-tag associations for a photo id and tag id.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo-tags.</returns>
        Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int photoId, int tagId);

        /// <summary>
        /// Associates a photo with a tag.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A new photo-tag entity.</returns>
        Task<PhotoTag> AssociatePhotoTag(int photoId, int tagId);

        /// <summary>
        /// Changes a photo tag association.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="existingTagId">The existing tag identifier.</param>
        /// <param name="newTagId">The new tag identifier.</param>
        /// <returns>A new photo-tag entity.</returns>
        Task<PhotoTag> AssociatePhotoTag(int photoId, int existingTagId, int newTagId);

        /// <summary>
        /// Dissociates a photo from a tag.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A void task.</returns>
        Task DissociatePhotoTag(int photoId, int tagId);

        /// <summary>
        /// Gets the tag by name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>The matching tag.</returns>
        Task<Tag> GetTag(string tagName);
    }
}