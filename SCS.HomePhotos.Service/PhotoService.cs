using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
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

        public PhotoService(IPhotoData photoData, ITagData tagData, ILogger<PhotoService> logger)
        {
            _photoData = photoData;
            _tagData = tagData;
            _logger = logger;
        }

        public async Task<Photo> GetPhotoByChecksum(string checksum)
        {
            var existing = await _photoData.GetListAsync<Photo>("WHERE Checksum = @Checksum", new { Checksum = checksum });

            return existing.FirstOrDefault();
        }

        public async Task<IEnumerable<Photo>> GetLatestPhotos(int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(DateTime.MinValue, DateTime.Today, true, pageNum, pageSize);
        }

        public async Task<IEnumerable<Photo>> GetPhotosByTag(string[] tags, int pageNum = 1, int pageSize = 200)
        {
            return await _photoData.GetPhotos(tags, pageNum, pageSize);
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

        public async Task<Tag> GetTag(string tagName)
        {
            var results = await _tagData.GetListAsync<Tag>("WHERE TagName = @TagName", new { TagName = tagName });

            return results.FirstOrDefault();
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
                await _photoData.AssociatePhotoTag(photo.PhotoId.Value, tag.TagId.Value);
            }
        }
    }
}
