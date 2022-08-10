﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Photo service.
    /// </summary>
    public interface IPhotoService
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
        /// <param name="dateTakenStart">The date taken start.</param>
        /// <param name="dateTakenEnd">The date taken end.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A paged list of photos.</returns>
        Task<IEnumerable<Photo>> GetPhotosByDateTaken(DateTime dateTakenStart, DateTime dateTakenEnd, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets the photos by tag.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A paged list of photos.</returns>
        Task<IEnumerable<Photo>> GetPhotosByTag(string[] tags, int pageNum = 0, int pageSize = 200);

        /// <summary>
        /// Gets the photos by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A paged list of photos.</returns>
        Task<IEnumerable<Photo>> GetPhotosByKeywords(string keywords, int pageNum = 1, int pageSize = 200);

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="includPhotoCounts">if set to <c>true</c> includ photo counts for each tag.</param>
        /// <returns>A list of tags.</returns>
        Task<IEnumerable<Tag>> GetTags(bool includPhotoCounts = false);

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="createIfMissing">if set to <c>true</c> create tag if missing.</param>
        /// <returns>A tag.</returns>
        Task<Tag> GetTag(string tagName, bool createIfMissing = true);

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>A tag.</returns>
        Task<Tag> GetTag(string tagName);

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
        Task<Tag> SaveTag(Tag tag);

        /// <summary>
        /// Saves a photo.
        /// </summary>
        /// <param name="photo">The photo to save.</param>
        /// <returns>A void task.</returns>
        Task SavePhoto(Photo photo);

        /// <summary>
        /// Associates tags with a photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A void task.</returns>
        Task AssociateTags(Photo photo, params string[] tags);

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
        /// <returns>The merged tag.</returns>
        Task<TagStat> MergeTags(string newTagName, params int[] targetTagIds);

        /// <summary>
        /// Copies a new tag with the same photo associations as another tag.
        /// </summary>
        /// <param name="newTagName">New name of the new tag.</param>
        /// <param name="sourceTagId">The tag to copy.</param>
        /// <returns>The new tag created.</returns>
        Task<Tag> CopyTags(string newTagName, int? sourceTagId);

        /// <summary>
        /// Gets tags by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of tags.</returns>
        Task<IEnumerable<Tag>> GetTagsByKeywords(string keywords, int pageNum, int pageSize);

        /// <summary>
        /// Gets the tags and photos of provided photo ids.
        /// </summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>A list of tags and their photos.</returns>
        Task<IEnumerable<Tag>> GetTagsAndPhotos(params int[] photoIds);

        /// <summary>
        /// Updates multiple photos with multiple tags.
        /// </summary>
        /// <param name="photoIds">The photo ids to update.</param>
        /// <param name="addTag">The tag to assign.</param>
        /// <param name="removeTagIds">The tag ids to be removed.</param>
        /// <returns>A void task.</returns>
        Task UpdatePhotoTags(List<int> photoIds, List<string> addTag, List<int> removeTagIds);

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
        Task DeletePhotoCache(string contextUserName);
    }
}