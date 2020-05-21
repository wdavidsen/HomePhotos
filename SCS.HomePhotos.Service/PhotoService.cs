using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public class PhotoService : IPhotoService
    {
        private readonly ILogger<PhotoService> _logger;

        private readonly IPhotoData _photoData;
        private readonly ITagData _tagData;
        private readonly IFileSystemService _fileSystemService;
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public PhotoService(IPhotoData photoData, ITagData tagData, ILogger<PhotoService> logger, IFileSystemService fileSystemService, 
            IDynamicConfig dynamicConfig, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _photoData = photoData;
            _tagData = tagData;
            _logger = logger;
            _fileSystemService = fileSystemService;
            _dynamicConfig = dynamicConfig;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public async Task<Photo> GetPhotoByChecksum(string checksum)
        {
            var existing = await _photoData.GetListAsync<Photo>("WHERE Checksum = @Checksum", new { Checksum = checksum });

            return existing.FirstOrDefault();
        }

        public async Task<IEnumerable<Photo>> GetLatestPhotos(int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(DateTime.MinValue, DateTime.Now, true, pageNum, pageSize);
        }

        public async Task<IEnumerable<Photo>> GetPhotosByTag(string[] tags, int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(tags, pageNum, pageSize);
        }

        public async Task<IEnumerable<Photo>> GetPhotosByKeywords(string keywords, int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(keywords, pageNum, pageSize);
        }

        public async Task<IEnumerable<Photo>> GetPhotosByDateTaken(DateTime dateTakenStart, DateTime dateTakenEnd, int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(dateTakenStart, dateTakenEnd, false, pageNum, pageSize);
        }

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

        public async Task<IEnumerable<Tag>> GetTagsByKeywords(string keywords, int pageNum, int pageSize)
        {
            return await _tagData.GetTags(keywords, pageNum, pageSize);
        }

        public async Task<Tag> GetTag(string tagName)
        {
            return await _tagData.GetTag(tagName);
        }

        public async Task DeleteTag(int tagId)
        {
            var tag = await _tagData.GetAsync<Tag>(tagId);

            if (tag == null)
            {
                throw new InvalidOperationException($"Tag id {tagId} was not found.");
            }
            await _tagData.DeleteAsync(tag);
        }

        public async Task<Tag> SaveTag(Tag tag)
        {
            return await _tagData.SaveTag(tag);
        }

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

        public async Task SavePhoto(Photo photo)
        {
            await _photoData.SavePhoto(photo);
        }

        public async Task AssociateTags(Photo photo, params string[] tags)
        {
            foreach (var tagName in tags)
            {
                var tag = await GetTag(tagName, true);
                await _tagData.AssociatePhotoTag(photo.PhotoId.Value, tag.TagId.Value);
            }
        }

        public async Task<Tag> MergeTags(string newTagName, params int[] targetTagIds)
        {
            var newTag = await GetTag(newTagName, true);

            foreach (var tagId in targetTagIds)
            {
                foreach (var assoc in await _tagData.GetPhotoTagAssociations(tagId))
                {
                    await _tagData.AssociatePhotoTag(assoc.PhotoId, assoc.TagId, newTag.TagId.Value);
                }
            }
            await DeleteUnusedTags();

            return newTag;
        }
        
        public async Task<Tag> CopyTags(string newTagName, int? sourceTagId)
        {
            var newTag = await GetTag(newTagName, true);

            foreach (var assoc in await _tagData.GetPhotoTagAssociations(sourceTagId.Value))
            {
                await _tagData.AssociatePhotoTag(assoc.PhotoId, newTag.TagId.Value);
            }

            return newTag;
        }

        public async Task<IEnumerable<Tag>> GetTagsAndPhotos(params int[] photoIds)
        {
            return await _photoData.GetTagsAndPhotos(photoIds);
        }

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

        private async Task<IEnumerable<Tag>> GetUnusedTags()
        {
            var tagStats = await _tagData.GetTagAndPhotoCount();

            return tagStats.Where(ts => ts.PhotoCount == 0);
        }

        private async Task DeleteUnusedTags()
        {
            foreach (var tag in await GetUnusedTags())
            {
                await _tagData.DeleteAsync(tag);
            }
        }

        public async Task FlagPhotosForReprocessing()
        {
            await _photoData.FlagPhotosForReprocessing();
        }

        public async Task DeletePhotoCache()
        {
            await _photoData.DeletePhotos();

            _backgroundTaskQueue.QueueBackgroundWorkItem((token) =>
            {
                try
                {
                    _fileSystemService.DeleteDirectoryFiles(_dynamicConfig.CacheFolder);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to delete cache folder: {_dynamicConfig.CacheFolder}");
                }
                return Task.CompletedTask;
            });            
        }
    }
}
