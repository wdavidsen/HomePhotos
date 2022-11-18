using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The tag repository.
    /// </summary>
    public interface ITagData : IDataBase<Tag>
    {
        /// <summary>
        /// Gets all tags.
        /// </summary>
        /// <returns>A list of all tags.</returns>
        Task<IEnumerable<Tag>> GetTags();

        /// <summary>
        /// Gets the tag by name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="userId">The owner of the tag.</param>
        /// <returns>The matching tag.</returns>
        Task<Tag> GetTag(string tagName, int? userId = null);

        /// <summary>
        /// Gets the tag and photo count.
        /// </summary>
        /// <returns>A list of tags.</returns>
        Task<IEnumerable<TagStat>> GetTagAndPhotoCount();

        /// <summary>
        /// Gets the tag and photo count.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="userId">Name user id owner.</param>
        /// <returns>The tag and photo count.</returns>
        Task<TagStat> GetTagAndPhotoCount(string tagName, int? userId);

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
        /// Saves a tag.
        /// </summary>
        /// <param name="tag">The tag to save.</param>
        /// <returns>The saved tag.</returns>
        Task<Tag> SaveTag(Tag tag);
    }
}