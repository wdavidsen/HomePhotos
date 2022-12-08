using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Photo service.
    /// </summary>
    public interface IPhotoService : IHomePhotosService
    {
        /// <summary>
        /// Gets the latest photos.
        /// </summary>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A paged list of photos.</returns>
        Task<IEnumerable<Photo>> GetLatestPhotos(int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets the photos by date taken.
        /// </summary>        
        /// <param name="dateRange">The specified date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        Task<IEnumerable<Photo>> GetPhotosByDate(DateRange dateRange, int pageNum = 1, int pageSize = 200);

        /// <summary>
        /// Gets the photos by tag.
        /// </summary>
        /// <param name="tag">The tags.</param>
        /// <param name="owner">The username owner of tags.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        Task<IEnumerable<Photo>> GetPhotosByTag(string tag, string owner, int pageNum = 1, int pageSize = 200);

        /// <summary>
        /// Gets the photos by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="dateRange">Optional date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A paged list of photos.</returns>
        Task<IEnumerable<Photo>> GetPhotosByKeywords(string keywords, DateRange? dateRange = null, int pageNum = 1, int pageSize = 200);

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="includPhotoCounts">if set to <c>true</c> include photo counts for each tag.</param>
        /// <param name="username">The owner user id of the tags. If set to null, shared tags will be returned.</param>
        /// <returns>A list of tags.</returns>
        Task<IEnumerable<Tag>> GetTags(string username = null, bool includPhotoCounts = false);

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="userId">The owner of the tag.</param>
        /// <param name="createIfMissing">if set to <c>true</c> create tag if missing.</param>
        /// <returns>A tag.</returns>
        Task<Tag> GetTag(string tagName, int? userId, bool createIfMissing);

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="userId">The owner of the tag.</param>
        /// <returns>A tag.</returns>
        Task<Tag> GetTag(string tagName, int? userId);

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A void task.</returns>
        Task DeleteTag(int tagId);

        /// <summary>
        /// Saves a tag.
        /// </summary>
        /// <param name="tag">The tag to save.</param>        
        /// <returns>The saved tag.</returns>
        Task<Tag> SaveTag(TagStat tag);

        /// <summary>
        /// Saves a photo.
        /// </summary>
        /// <param name="photo">The photo to save.</param>
        /// <returns>A void task.</returns>
        Task SavePhoto(Photo photo);

        /// <summary>
        /// Deletes a photo and its image files.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <exception cref="System.InvalidOperationException">Photo id {photoId} was not found.</exception>
        Task DeletePhoto(int photoId);

        /// <summary>
        /// Associates tags with a photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A void task.</returns>
        Task AssociateTags(Photo photo, IEnumerable<Tag> tags);

        /// <summary>
        /// Associates the user to photo from tags if possible.
        /// </summary>
        /// <param name="photo">The photo.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A void task.</returns>
        Task AssociateUser(Photo photo, List<Tag> tags);

        /// <summary>
        /// Gets a photo by checksum.
        /// </summary>
        /// <param name="checksum">The photo file checksum.</param>
        /// <returns>A photo.</returns>
        Task<Photo> GetPhotoByChecksum(string checksum);

        /// <summary>
        /// Merges several tags.
        /// </summary>
        /// <param name="newTagName">New name of the tag.</param>
        /// <param name="targetTagIds">The target tag ids.</param>
        /// <param name="ownerId">The owner user id of new merged tag.</param>
        /// <returns>
        /// The merged tag.
        /// </returns>
        Task<TagStat> MergeTags(string newTagName, int[] targetTagIds, int? ownerId);

        /// <summary>
        /// Copies a new tag with the same photo associations as another tag.
        /// </summary>
        /// <param name="newTagName">New name of the new tag.</param>
        /// <param name="sourceTagId">The tag to copy.</param>        
        /// <param name="ownerId">The owner of the new tag.</param>       
        /// <returns>The new tag created.</returns>
        Task<TagStat> CopyTags(string newTagName, int? sourceTagId, int? ownerId);

        /// <summary>
        /// Gets tags by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="dateRange">The optional date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A list of tags.
        /// </returns>
        Task<IEnumerable<Tag>> GetTagsByKeywords(string keywords, DateRange? dateRange, int pageNum, int pageSize);

        /// <summary>
        /// Gets the tags by via date range.
        /// </summary>
        /// <param name="dateRange">The date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A list of tags.
        /// </returns>
        Task<IEnumerable<Tag>> GetTagsByDate(DateRange dateRange, int pageNum, int pageSize);

        /// <summary>
        /// Gets the tags and photos of provided photo ids.
        /// </summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <param name="username">The owner username of the tags.</param>
        /// <returns>A list of tags and their photos.</returns>
        Task<IEnumerable<Tag>> GetTagsAndPhotos(string username, params int[] photoIds);

        /// <summary>
        /// Updates multiple photos with multiple tags.
        /// </summary>
        /// <param name="username">The owner username of the tags.</param>
        /// <param name="photoIds">The photo ids to update.</param>
        /// <param name="addTag">The tag to assign.</param>
        /// <param name="removeTagIds">The tag ids to be removed.</param>
        /// <returns>A void task.</returns>
        Task UpdatePhotoTags(string username, List<int> photoIds, List<string> addTag, List<int> removeTagIds);

        /// <summary>
        /// Flags photos for reprocessing.
        /// </summary>
        /// <returns></returns>
        Task FlagPhotosForReprocessing();

        /// <summary>
        /// Deletes the entire photo file cache.
        /// </summary>
        /// <param name="contextUserName">Name of the context user.</param>
        /// <returns>A void task.</returns>
        Task ResetPhotosAndTags(string contextUserName);

        /// <summary>
        /// Deletes all photos under a specific directory.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> directory is under mobile uploads folder.</param>
        /// <param name="originalFolder">The original folder relative to the index/mobile uploads folder.</param>
        /// <returns>A void task.</returns>
        Task DeleteDirectoryPhotos(bool mobileUpload, string originalFolder);

        /// <summary>
        /// Deletes a photo by its original index location.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> photo was a mobile upload.</param>
        /// <param name="originalFolder">The original folder path relative to the index/mobile upload folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A void task.</returns>
        Task DeletePhoto(bool mobileUpload, string originalFolder, string fileName);        
    }
}