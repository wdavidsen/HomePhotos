﻿using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

using SCS.HomePhotos.Data;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Workers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Photo services.
    /// </summary>
    public class PhotoService : HomePhotosService, IPhotoService
    {
        private readonly ILogger<PhotoService> _logger;

        private readonly IPhotoData _photoData;
        private readonly ITagData _tagData;
        private readonly IPhotoTagData _photoTagData;
        private readonly IFileExclusionData _fileExclusionData;
        private readonly IUserData _userData;
        private readonly IUserSettingsData _userSettingsData;
        private readonly IFileSystemService _fileSystemService;
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IAdminLogService _adminLogService;

        private readonly string _noiseWords = "null";
        private readonly string _sysTagColor = Constants.DefaultTagColor;

        /// <summary>
        /// The photo service.
        /// </summary>
        /// <param name="photoData">The photo data repository.</param>
        /// <param name="tagData">The tag data repository.</param>
        /// <param name="photoTagData">The photo tag data repository.</param>
        /// <param name="fileExclusionData">The file exclusion data repository.</param>
        /// <param name="userData">The user data repository.</param>
        /// <param name="userSettingsData">The user settings data repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="adminLogger">The admin logger.</param>
        /// <param name="fileSystemService">The file system service.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="backgroundTaskQueue">The background task queue.</param>
        public PhotoService(IPhotoData photoData, ITagData tagData, IPhotoTagData photoTagData, IFileExclusionData fileExclusionData, IUserData userData, IUserSettingsData userSettingsData,
            ILogger<PhotoService> logger, IAdminLogService adminLogger, IFileSystemService fileSystemService, IDynamicConfig dynamicConfig, IBackgroundTaskQueue backgroundTaskQueue)
            : base(dynamicConfig, userData, userSettingsData)
        {
            _photoData = photoData;
            _tagData = tagData;
            _photoTagData = photoTagData;
            _fileExclusionData = fileExclusionData;
            _userData = userData;
            _userSettingsData = userSettingsData;
            _logger = logger;
            _adminLogService = adminLogger;
            _fileSystemService = fileSystemService;
            _dynamicConfig = dynamicConfig;
            _backgroundTaskQueue = backgroundTaskQueue;

            _sysTagColor = dynamicConfig.TagColor;
        }

        /// <summary>
        /// Gets a photo by checksum.
        /// </summary>
        /// <param name="checksum">The photo file checksum.</param>
        /// <returns>
        /// A photo.
        /// </returns>
        public async Task<Photo> GetPhotoByChecksum(string checksum)
        {
            var existing = await _photoData.GetListAsync("WHERE Checksum = @Checksum", new { Checksum = checksum });

            return existing.FirstOrDefault();
        }

        /// <summary>
        /// Gets the latest photos.
        /// </summary>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        public async Task<IEnumerable<Photo>> GetLatestPhotos(int pageNum = 1, int pageSize = 200)
        {
            var scope = await GetViewScope(User.Identity.Name);

            var userFilter = new UserFilter(scope.Scope, await GetFilterUserId(scope.OwnerUsername));

            return await _photoData.GetPhotos(userFilter, new DateRange(DateTime.Now, DateTime.MinValue), pageNum, pageSize);
        }

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
        public async Task<IEnumerable<Photo>> GetPhotosByTag(string tag, string owner, int pageNum = 1, int pageSize = 200)
        {
            var scope = await GetViewScope(owner);

            var userFilter = new UserFilter(scope.Scope, await GetFilterUserId(scope.OwnerUsername));

            return await _photoData.GetPhotos(userFilter, tag, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the photos by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="username">The username filter (optional).</param>
        /// <param name="dateRange">Optional date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        public async Task<IEnumerable<Photo>> GetPhotosByKeywords(string keywords, string username, DateRange dateRange, int pageNum = 1, int pageSize = 200)
        {
            var scope = await GetViewScope(username);

            var userFilter = new UserFilter(scope.Scope, await GetFilterUserId(scope.OwnerUsername));

            return await _photoData.GetPhotos(userFilter, dateRange, keywords, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the photos by date taken.
        /// </summary>        
        /// <param name="dateRange">The specified date range.</param>
        /// <param name="username">The username filter (optional).</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        public async Task<IEnumerable<Photo>> GetPhotosByDate(DateRange dateRange, string username, int pageNum = 1, int pageSize = 200)
        {
            var scope = await GetViewScope(username);

            var userFilter = new UserFilter(scope.Scope, await GetFilterUserId(scope.OwnerUsername));

            return await _photoData.GetPhotos(userFilter, dateRange, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="includPhotoCounts">if set to <c>true</c> include photo counts for each tag.</param>
        /// <param name="username">The owner username of the tags. If set to null, shared tags will be returned.</param>
        /// <returns>A list of tags.</returns>
        public async Task<IEnumerable<Tag>> GetTags(string username = null, bool includPhotoCounts = false)
        {
            var scope = await GetViewScope(username);

            var userFilter = new UserFilter(scope.Scope, await GetFilterUserId(scope.OwnerUsername));

            if (includPhotoCounts)
            {
                return AssignSysTagColor(await _tagData.GetTagAndPhotoCount(userFilter));
            }
            else
            {
                return await _tagData.GetTags(userFilter);
            }
        }

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
        public async Task<IEnumerable<Tag>> GetTagsByKeywords(string keywords, DateRange? dateRange, int pageNum, int pageSize)
        {
            return AssignSysTagColor(await _tagData.GetTags(keywords, dateRange, pageNum, pageSize));
        }

        /// <summary>
        /// Gets the tags by via date range.
        /// </summary>
        /// <param name="dateRange">The date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A list of tags.
        /// </returns>
        public async Task<IEnumerable<Tag>> GetTagsByDate(DateRange dateRange, int pageNum, int pageSize)
        {
            return AssignSysTagColor(await _tagData.GetTags(dateRange, pageNum, pageSize));
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="userId">The owner of the tag.</param>
        /// <returns>A tag.</returns>
        public async Task<Tag> GetTag(string tagName, int? userId)
        {
            return await _tagData.GetTag(tagName, userId);
        }

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <exception cref="System.InvalidOperationException">Tag id {tagId} was not found.</exception>
        public async Task DeleteTag(int tagId)
        {
            var tag = await _tagData.GetAsync(tagId);
            var currentUser = await GetCurrentUser();

            if (currentUser.Role != RoleType.Admin && tag.UserId != currentUser.UserId)
            {
                throw new AccessException("Other users's tags cannot be deleted, without Admin rights.");
            }

            if (tag == null)
            {
                throw new InvalidOperationException($"Tag id {tagId} was not found.");
            }
            await _tagData.DeleteAsync(tag);
        }

        /// <summary>
        /// Deletes a photo and its image files.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <exception cref="System.InvalidOperationException">Photo id {photoId} was not found.</exception>
        public async Task DeletePhoto(int photoId)
        {
            var photo = await _photoData.GetAsync(photoId);

            if (photo == null)
            {
                throw new InvalidOperationException($"Photo id {photoId} was not found.");
            }
            await _photoData.DeleteAsync(photo);

            DeleteCacheImages(photo.CacheFolder, photo.FileName);

            var insertSkip = true;

            if (photo.MobileUpload)
            {
                if (_dynamicConfig.MobilePhotoDeleteAction == DeleteAction.DeleteRecordAndFile)
                {
                    insertSkip = !DeleteOriginalImage(true, photo.OriginalFolder, photo.Name);
                }
            }
            else
            {
                if (_dynamicConfig.PhotoDeleteAction == DeleteAction.DeleteRecordAndFile)
                {
                    insertSkip = !DeleteOriginalImage(false, photo.OriginalFolder, photo.Name);
                }
            }

            if (insertSkip && !string.IsNullOrWhiteSpace(photo.OriginalFolder))
            {
                await _fileExclusionData.InsertAsync(
                    new FileExclusion
                    {
                        MobileUpload = photo.MobileUpload,
                        OriginalFolder = photo.OriginalFolder,
                        FileName = photo.Name
                    });
            }

            foreach (var tag in await GetUnusedTags())
            {
                await _tagData.DeleteAsync(tag);
            }
        }

        /// <summary>
        /// Saves a tag.
        /// </summary>
        /// <param name="tag">The tag to save.</param>
        /// <returns>
        /// The saved tag.
        /// </returns>
        public async Task<Tag> SaveTag(TagStat tag)
        {
            var isAdd = tag.TagId == null || tag.TagId == 0;
            var currentUser = await GetCurrentUser();
            var isAdmin = currentUser.Role == RoleType.Admin;

            if (isAdd && tag.UserId == null && !isAdmin)
            {
                throw new AccessException("Cannot create shared tag, without Admin rights.");
            }

            return await _tagData.SaveTag(tag);
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="userId">The owner of the tag.</param>
        /// <param name="createIfMissing">if set to <c>true</c> create tag if missing.</param>
        /// <returns>
        /// A tag.
        /// </returns>
        public async Task<Tag> GetTag(string tagName, int? userId, bool createIfMissing)
        {
            var existing = await GetTag(tagName, userId);

            if (existing != null)
            {
                return existing;
            }

            if (!createIfMissing)
            {
                return null;
            }

            var tag = new Tag
            {
                TagName = tagName,
                UserId = userId
            };

            try
            {
                return await _tagData.SaveTag(tag);
            }
            catch (SqliteException ex)
            {
                // unique constraint race condition?
                if (ex.ErrorCode == 19)
                {
                    return await GetTag(tagName, userId);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Saves a photo.
        /// </summary>
        /// <param name="photo">The photo to save.</param>
        public async Task SavePhoto(Photo photo)
        {
            await _photoData.SavePhoto(photo);
        }

        /// <summary>
        /// Associates tags with a photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        /// <param name="tags">The tags.</param>
        public async Task AssociateTags(Photo photo, IEnumerable<Tag> tags)
        {
            var noise = _noiseWords.Split(',');

            foreach (var tag in tags)
            {
                if (noise.Any(w => w.ToUpper() == tag.TagName)) continue;

                var existingTag = await GetTag(tag.TagName, tag.UserId, true);
                await _photoTagData.AssociatePhotoTag(photo.PhotoId.Value, existingTag.TagId.Value);
            }
        }

        /// <summary>
        /// Associates the user to photo from tags if possible.
        /// </summary>
        /// <param name="imageFileSource">The source of the image file.</param>
        /// <param name="owner">The owner of the photo.</param>
        /// <param name="photo">The photo.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A void task.</returns>
        public async Task SetUserId(ImageFileSource imageFileSource, User owner, Photo photo, List<Tag> tags)
        {
            switch (imageFileSource)
            {
                case ImageFileSource.LocalDisk:

                    if (tags != null && tags.Any())
                    {
                        var users = await _userData.GetListAsync();
                        var user = users.FirstOrDefault(u => u.UserName.Equals(tags.First().TagName, StringComparison.InvariantCultureIgnoreCase));

                        if (user != null)
                        {
                            photo.UserId = user.UserId;
                            tags.Skip(1).ToList().ForEach(t => t.UserId = photo.UserId);
                        }
                    }
                    break;

                case ImageFileSource.MobileUpload:

                    photo.UserId = owner?.UserId;
                    break;
            }
        }

        /// <summary>
        /// Merges several tags.
        /// </summary>
        /// <param name="newTagName">New name of the tag.</param>
        /// <param name="targetTagIds">The target tag ids.</param>
        /// <param name="ownerId">The owner user id of new merged tag.</param>
        /// <returns>
        /// The merged tag.
        /// </returns>
        public async Task<TagStat> MergeTags(string newTagName, int[] targetTagIds, int? ownerId)
        {
            var tagsToDelete = new List<string>();
            var currentUser = await GetCurrentUser();

            var tagAssoc = new List<UserPhotoTag>();

            foreach (var tagId in targetTagIds)
            {
                tagAssoc.AddRange(await _photoTagData.GetPhotoTagAssociations(tagId));
            }

            var uniqueUsers = tagAssoc.Select(pt => pt.UserId).Distinct();

            if (uniqueUsers.Any(u => u != currentUser.UserId) && currentUser.Role != RoleType.Admin)
            {
                throw new AccessException("Cannot merge these tags without Admin rights.");
            }

            var newTag = await GetTag(newTagName, ownerId, true);

            foreach (var assoc in tagAssoc)
            {
                await _photoTagData.AssociatePhotoTag(assoc.PhotoId, assoc.TagId, newTag.TagId.Value);
            }
            await DeleteUnusedTags();

            return await _tagData.GetTagAndPhotoCount(newTagName, ownerId);
        }

        /// <summary>
        /// Copies a new tag with the same photo associations as another tag.
        /// </summary>
        /// <param name="newTagName">New name of the new tag.</param>
        /// <param name="sourceTagId">The tag to copy.</param>
        /// <param name="ownerId">The owner of the new tag.</param>   
        /// <returns>
        /// The new tag created.
        /// </returns>
        public async Task<Model.TagStat> CopyTags(string newTagName, int? sourceTagId, int? ownerId)
        {
            var currentUser = await GetCurrentUser();
            var isAdmin = currentUser.Role == RoleType.Admin;

            if (ownerId == null && !isAdmin)
            {
                throw new InvalidOperationException("Cannot create shared tag, without Admin rights.");
            }

            var newTag = await GetTag(newTagName, ownerId, true);

            foreach (var assoc in await _photoTagData.GetPhotoTagAssociations(sourceTagId.Value))
            {
                await _photoTagData.AssociatePhotoTag(assoc.PhotoId, newTag.TagId.Value);
            }

            var tagStat = await _tagData.GetTagAndPhotoCount(newTag.TagName, ownerId);

            return tagStat;
        }

        /// <summary>
        /// Gets the tags and photos of provided photo ids.
        /// </summary>
        /// <param name="username">The owner username of the tags.</param>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>
        /// A list of tags and their photos.
        /// </returns>
        public async Task<IEnumerable<Model.Tag>> GetTagsAndPhotos(string username, params int[] photoIds)
        {
            var user = await _userData.GetUser(username);

            return await _photoData.GetTagsAndPhotos(photoIds, user?.UserId);
        }

        /// <summary>
        /// Updates the photo tags.
        /// </summary>
        /// <param name="username">The owner username of the tags.</param>
        /// <param name="photoIds">The photo ids.</param>
        /// <param name="addTagNames">The add tag names.</param>
        /// <param name="removeTagIds">The remove tag ids.</param>
        public async Task UpdatePhotoTags(string username, List<int> photoIds, List<string> addTagNames, List<int> removeTagIds)
        {
            var user = await _userData.GetUser(username);
            var photos = await _photoData.GetPhotosAndTags(user?.UserName, photoIds.ToArray());

            foreach (var photo in photos)
            {
                foreach (var addTagName in addTagNames)
                {
                    if (!photo.Tags.Any(t => t.TagName == addTagName))
                    {
                        var tag = await GetTag(addTagName, user?.UserId, true);

                        if (tag != null)
                        {
                            photo.Tags.Add(tag);
                            await _photoTagData.AssociatePhotoTag(photo.PhotoId.Value, tag.TagId.Value);
                        }
                    }
                }

                foreach (var removeTagId in removeTagIds)
                {
                    if (photo.Tags.Any(t => t.TagId == removeTagId))
                    {
                        var tag = await _tagData.GetAsync(removeTagId);

                        if (tag != null)
                        {
                            photo.Tags.Remove(tag);
                            await _photoTagData.DissociatePhotoTag(photo.PhotoId.Value, tag.TagId.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Flags photos for reprocessing.
        /// </summary>
        public async Task FlagPhotosForReprocessing()
        {
            _adminLogService.LogElevated($"Image index has been manually triggered by {User.Identity.Name}.", LogCategory.Index);

            await _photoData.FlagPhotosForReprocessing();
        }

        /// <summary>
        /// Deletes the entire photo file cache.
        /// </summary>
        /// <param name="contextUserName">Name of the context user.</param>
        public async Task ResetPhotosAndTags(string contextUserName)
        {
            _adminLogService.LogHigh($"Factory reset has been triggered by {User.Identity.Name}.", LogCategory.Security);
            await _photoData.DeletePhotos();

            _backgroundTaskQueue.QueueBackgroundWorkItem((token, notifier) =>
            {
                return Task.Run(() =>
                {

                    try
                    {
                        _fileSystemService.DeleteDirectoryFiles(_dynamicConfig.CacheFolder, true, true);
                        notifier.ItemProcessed(new TaskCompleteInfo(TaskType.ClearCache, contextUserName, true));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete cache folder: {_dynamicConfig.CacheFolder}", _dynamicConfig.CacheFolder);
                        notifier.ItemProcessed(new TaskCompleteInfo(TaskType.ClearCache, contextUserName, false));
                    }
                });
            });
        }

        /// <summary>
        /// Deletes all photos under a specific directory.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> directory is under mobile uploads folder.</param>
        /// <param name="originalFolder">The original folder relative to the index/mobile uploads folder.</param>
        /// <returns>A void task.</returns>
        public async Task DeleteDirectoryPhotos(bool mobileUpload, string originalFolder)
        {
            var dirPhotos = await _photoData.GetListAsync("WHERE MobileUpload = @MobileUpload AND OriginalFolder = @OriginalFolder",
                new { MobileUpload = mobileUpload, OriginalFolder = originalFolder });

            foreach (var photo in dirPhotos)
            {
                await DeletePhoto(photo.PhotoId.Value);
            }
        }

        /// <summary>
        /// Deletes a photo by its original index location.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> photo was a mobile upload.</param>
        /// <param name="originalFolder">The original folder path relative to the index/mobile upload folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A void task.</returns>
        public async Task DeletePhoto(bool mobileUpload, string originalFolder, string fileName)
        {
            var photos = await _photoData.GetListAsync("WHERE MobileUpload = @MobileUpload AND OriginalFolder = @OriginalFolder AND FileName = @FileName",
                new { MobileUpload = mobileUpload, OriginalFolder = originalFolder, FileName = fileName });

            foreach (var photo in photos)
            {
                await DeletePhoto(photo.PhotoId.Value);
            }
        }

        private async Task<IEnumerable<Tag>> GetUnusedTags()
        {
            var tagStats = await _tagData.GetTagAndPhotoCount(new UserFilter());

            return tagStats.Where(ts => ts.PhotoCount == 0);
        }

        private async Task DeleteUnusedTags()
        {
            foreach (var tag in await GetUnusedTags())
            {
                await _tagData.DeleteAsync(tag);
            }
        }

        /// <summary>
        /// Deletes the cache images of a photo.
        /// </summary>
        /// <param name="cacheSubFolder">The image cache folder.</param>
        /// <param name="fileName">The image file name.</param>
        private bool DeleteCacheImages(string cacheSubFolder, string fileName)
        {
            try
            {
                _fileSystemService.DeleteImageFile(_dynamicConfig.CacheFolder, $"{cacheSubFolder}\\{ImageSizeType.Thumb}", fileName);
                _fileSystemService.DeleteImageFile(_dynamicConfig.CacheFolder, $"{cacheSubFolder}\\{ImageSizeType.Small}", fileName);
                _fileSystemService.DeleteImageFile(_dynamicConfig.CacheFolder, $"{cacheSubFolder}\\{ImageSizeType.Full}", fileName);
                return true;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Failed to delete cache images from file system.");
                return false;
            }
        }

        /// <summary>
        /// Deletes the original photo image.
        /// </summary>
        /// <param name="isMobile">if set to <c>true</c> photo is a mobile upload.</param>
        /// <param name="subfolderName">Name of the sub-folder.</param>
        /// <param name="fileName">Name of the file.</param>
        private bool DeleteOriginalImage(bool isMobile, string subfolderName, string fileName)
        {
            try
            {
                var baseFolder = isMobile ? _dynamicConfig.MobileUploadsFolder : _dynamicConfig.IndexPath;
                _fileSystemService.DeleteImageFile(baseFolder, subfolderName, fileName);
                return true;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Failed to delete original image from file system.");
                return false;
            }
        }

        private IEnumerable<Tag> AssignSysTagColor(IEnumerable<TagStat> tags)
        {
            tags.ToList().ForEach(t => t.TagColor ??= _sysTagColor);
            return tags;
        }

        private async Task<User> GetCurrentUser()
        {
            return await _userData.GetUser(User.Identity.Name);
        }

        private async Task<int?> GetFilterUserId(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return (await GetCurrentUser()).UserId;
            }
            var user = await _userData.GetUser(userName);

            if (user == null)
            {
                throw new EntityNotFoundException($"User '{userName}' not found.");
            }
            return user.UserId.Value;
        }
    }
}
