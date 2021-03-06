﻿using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The photo repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IDataBase" />
    public interface IPhotoData : IDataBase
    {
        /// <summary>
        /// Gets a list of photos matching search criteria.
        /// </summary>
        /// <param name="dateTakenStart">The date taken start timestamp.</param>
        /// <param name="dateTakenEnd">The date taken end timestamp.</param>
        /// <param name="descending">if set to <c>true</c> sort descending; otherwise, sort ascending.</param>
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Photo>> GetPhotos(DateTime dateTakenStart, DateTime dateTakenEnd, bool descending = true, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets a list of photos by tag.
        /// </summary>
        /// <param name="tags">The tags to search by.</param>
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Photo>> GetPhotos(string[] tags, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets a list of photos by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        Task<IEnumerable<Photo>> GetPhotos(string keywords, int pageNum = 0, int pageSize = 200);

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
    }
}