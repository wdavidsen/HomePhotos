using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Data.Core;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static System.Net.WebRequestMethods;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Photo services.
    /// </summary>
    public class PhotoService : IPhotoService
    {
        private readonly ILogger<PhotoService> _logger;

        private readonly IPhotoData _photoData;
        private readonly ITagData _tagData;
        private readonly ISkipImageData _skipImageData;
        private readonly IFileSystemService _fileSystemService;
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        private string _noiseWords = "null";

        /// <summary>Initializes a new instance of the <see cref="PhotoService" /> class.</summary>
        /// <param name="photoData">The photo data.</param>
        /// <param name="tagData">The tag data.</param>
        /// <param name="skipImageData">The skip image data.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="fileSystemService">The file system service.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="backgroundTaskQueue">The background task queue.</param>
        public PhotoService(IPhotoData photoData, ITagData tagData, ISkipImageData skipImageData, ILogger<PhotoService> logger, IFileSystemService fileSystemService,
            IDynamicConfig dynamicConfig, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _photoData = photoData;
            _tagData = tagData;
            _skipImageData = skipImageData;
            _logger = logger;
            _fileSystemService = fileSystemService;
            _dynamicConfig = dynamicConfig;
            _backgroundTaskQueue = backgroundTaskQueue;
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
            var existing = await _photoData.GetListAsync<Photo>("WHERE Checksum = @Checksum", new { Checksum = checksum });

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
            return await _photoData.GetPhotos(DateTime.MinValue, DateTime.Now, true, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the photos by tag.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        public async Task<IEnumerable<Photo>> GetPhotosByTag(string[] tags, int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(tags, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the photos by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        public async Task<IEnumerable<Photo>> GetPhotosByKeywords(string keywords, int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(keywords, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the photos by date taken.
        /// </summary>
        /// <param name="dateTakenStart">The date taken start.</param>
        /// <param name="dateTakenEnd">The date taken end.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A paged list of photos.
        /// </returns>
        public async Task<IEnumerable<Photo>> GetPhotosByDateTaken(DateTime dateTakenStart, DateTime dateTakenEnd, int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(dateTakenStart, dateTakenEnd, false, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="includPhotoCounts">if set to <c>true</c> includ photo counts for each tag.</param>
        /// <returns>A list of tags.</returns>
        public async Task<IEnumerable<Tag>> GetTags(bool includPhotoCounts = false)
        {
            if (includPhotoCounts)
            {
                return await _tagData.GetTagAndPhotoCount();
            }
            else
            {
                return await _tagData.GetTags();
            }
        }

        /// <summary>
        /// Gets tags by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A list of tags.
        /// </returns>
        public async Task<IEnumerable<Tag>> GetTagsByKeywords(string keywords, int pageNum, int pageSize)
        {
            return await _tagData.GetTags(keywords, pageNum, pageSize);
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>A tag.</returns>
        public async Task<Tag> GetTag(string tagName)
        {
            return await _tagData.GetTag(tagName);
        }

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <exception cref="System.InvalidOperationException">Tag id {tagId} was not found.</exception>
        public async Task DeleteTag(int tagId)
        {
            var tag = await _tagData.GetAsync<Tag>(tagId);

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
            var photo = await _photoData.GetAsync<Photo>(photoId);

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
                await _skipImageData.InsertAsync(
                    new SkipImage
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
        public async Task<Tag> SaveTag(Tag tag)
        {
            return await _tagData.SaveTag(tag);
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="createIfMissing">if set to <c>true</c> create tag if missing.</param>
        /// <returns>
        /// A tag.
        /// </returns>
        public async Task<Tag> GetTag(string tagName, bool createIfMissing = true)
        {
            var existing = await GetTag(tagName);

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
                TagName = tagName
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
                    return await GetTag(tagName);
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
        public async Task AssociateTags(Photo photo, params string[] tags)
        {
            var noise = _noiseWords.Split(',');

            foreach (var tagName in tags)
            {
                if (noise.Any(w => w.ToUpper() == tagName.ToUpper())) continue;

                var tag = await GetTag(tagName, true);
                await _tagData.AssociatePhotoTag(photo.PhotoId.Value, tag.TagId.Value);
            }
        }

        /// <summary>
        /// Merges several tags.
        /// </summary>
        /// <param name="newTagName">New name of the tag.</param>
        /// <param name="targetTagIds">The target tag ids.</param>
        /// <returns>
        /// The merged tag.
        /// </returns>
        public async Task<TagStat> MergeTags(string newTagName, params int[] targetTagIds)
        {
            var newTag = await GetTag(newTagName, true);
            var tagsToDelete = new List<string>();

            foreach (var tagId in targetTagIds)
            {
                foreach (var assoc in await _tagData.GetPhotoTagAssociations(tagId))
                {
                    await _tagData.AssociatePhotoTag(assoc.PhotoId, assoc.TagId, newTag.TagId.Value);
                    var tag = await _tagData.GetAsync<Tag>(tagId);

                    if (!tag.TagName.Equals(newTagName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        tagsToDelete.Add(tag.TagName);
                    }
                }
            }
            await DeleteUnusedTags(tagsToDelete.ToArray());

            var tagStat = await _tagData.GetTagAndPhotoCount(newTagName);

            return tagStat;
        }

        /// <summary>
        /// Copies a new tag with the same photo associations as another tag.
        /// </summary>
        /// <param name="newTagName">New name of the new tag.</param>
        /// <param name="sourceTagId">The tag to copy.</param>
        /// <returns>
        /// The new tag created.
        /// </returns>
        public async Task<Tag> CopyTags(string newTagName, int? sourceTagId)
        {
            var newTag = await GetTag(newTagName, true);

            foreach (var assoc in await _tagData.GetPhotoTagAssociations(sourceTagId.Value))
            {
                await _tagData.AssociatePhotoTag(assoc.PhotoId, newTag.TagId.Value);
            }

            return newTag;
        }

        /// <summary>
        /// Gets the tags and photos of provided photo ids.
        /// </summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>
        /// A list of tags and their photos.
        /// </returns>
        public async Task<IEnumerable<Tag>> GetTagsAndPhotos(params int[] photoIds)
        {
            return await _photoData.GetTagsAndPhotos(photoIds);
        }

        /// <summary>
        /// Updates the photo tags.
        /// </summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <param name="addTagNames">The add tag names.</param>
        /// <param name="removeTagIds">The remove tag ids.</param>
        public async Task UpdatePhotoTags(List<int> photoIds, List<string> addTagNames, List<int> removeTagIds)
        {
            var photos = await _photoData.GetPhotosAndTags(photoIds.ToArray());

            foreach (var photo in photos)
            {
                foreach (var addTagName in addTagNames)
                {
                    if (!photo.Tags.Any(t => t.TagName == addTagName))
                    {
                        var tag = await GetTag(addTagName, true);

                        if (tag != null)
                        {
                            photo.Tags.Add(tag);
                            await _tagData.AssociatePhotoTag(photo.PhotoId.Value, tag.TagId.Value);
                        }
                    }
                }

                foreach (var removeTagId in removeTagIds)
                {
                    if (photo.Tags.Any(t => t.TagId == removeTagId))
                    {
                        var tag = await _tagData.GetAsync<Tag>(removeTagId);

                        if (tag != null)
                        {
                            photo.Tags.Remove(tag);
                            await _tagData.DissociatePhotoTag(photo.PhotoId.Value, tag.TagId.Value);
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
            await _photoData.FlagPhotosForReprocessing();
        }

        /// <summary>
        /// Deletes the entire photo file cache.
        /// </summary>
        /// <param name="contextUserName">Name of the context user.</param>
        public async Task DeletePhotoCache(string contextUserName)
        {
            await _photoData.DeletePhotos();

            _backgroundTaskQueue.QueueBackgroundWorkItem((token, notifier) =>
            {
                return Task.Run(() =>
                {

                    try
                    {
                        _fileSystemService.DeleteDirectoryFiles(_dynamicConfig.CacheFolder);
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

        private async Task<IEnumerable<Tag>> GetUnusedTags()
        {
            var tagStats = await _tagData.GetTagAndPhotoCount();

            return tagStats.Where(ts => ts.PhotoCount == 0);
        }

        private async Task DeleteUnusedTags(params string[] tagNames)
        {
            foreach (var tag in await GetUnusedTags())
            {
                if (tagNames.Any(n => n == tag.TagName))
                {
                    await _tagData.DeleteAsync(tag);
                }
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
        /// <param name="subfolderName">Name of the subfolder.</param>
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
    }
}
