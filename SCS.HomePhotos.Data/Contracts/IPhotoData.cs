using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The photo repository.
    /// </summary>
    public interface IPhotoData : IDataBase<Photo>
    {
        /// <summary>
        /// Gets a list of photos matching search criteria.
        /// </summary>        
        /// <param name="dateRange">The date taken range.</param>        
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Photo>> GetPhotos(DateRange dateRange, int pageNum = 1, int pageSize = 200);

        /// <summary>
        /// Gets a list of photos by tag.
        /// </summary>
        /// <param name="tag">The tag to search on.</param>
        /// <param name="ownerId">The user id owner of tags.</param>
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Photo>> GetPhotos(string tag, int? ownerId, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets a list of photos by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="dateRange">The optional date range.</param>
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Photo>> GetPhotos(string keywords, DateRange? dateRange = null, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets a list of photos by photo ids.
        /// </summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Tag>> GetTagsAndPhotos(int[] photoIds);

        /// <summary>
        /// Gets a list of photos and tags by photo ids.
        /// </summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Photo>> GetPhotosAndTags(params int[] photoIds);

        /// <summary>
        /// Saves a photo entity.
        /// </summary>
        /// <param name="photo">The photo entity.</param>
        /// <returns>The saved photo entity.</returns>
        Task<Photo> SavePhoto(Photo photo);

        /// <summary>
        /// Flags all photos for reprocessing.
        /// </summary>
        /// <returns>A void task.</returns>
        Task FlagPhotosForReprocessing();

        /// <summary>
        /// Deletes all photos.
        /// </summary>
        /// <returns>A void task.</returns>
        Task DeletePhotos();

        /// <summary>
        /// Deletes a photo by its original index location.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> photo was a mobile upload.</param>
        /// <param name="originalFolder">The original folder path relative to the index/mobile upload folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A void task.</returns>
        Task DeletePhoto(bool mobileUpload, string originalFolder, string fileName);

        /// <summary>
        /// Deletes all photos under a specific directory.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> directory is under mobile uploads folder.</param>
        /// <param name="originalFolder">The original folder relative to the index/mobile uploads folder.</param>
        Task DeleteDirectoryPhotos(bool mobileUpload, string originalFolder);
    }
}