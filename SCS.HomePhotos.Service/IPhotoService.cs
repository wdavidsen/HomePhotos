using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Service
{
    public interface IPhotoService
    {
        Task<IEnumerable<Photo>> GetLatestPhotos(int pageNum = 0, int pageSize = 200);
        Task<IEnumerable<Photo>> GetPhotosByDateTaken(DateTime dateTakenStart, DateTime dateTakenEnd, int pageNum = 0, int pageSize = 200);
        Task<IEnumerable<Photo>> GetPhotosByTag(string[] tags, int pageNum = 0, int pageSize = 200);
        Task<IEnumerable<Photo>> GetPhotosByKeywords(string keywords, int pageNum = 1, int pageSize = 200);
        Task<IEnumerable<Tag>> GetTags(bool includPhotoCounts = false);
        Task<Tag> GetTag(string tagName, bool createIfMissing = true);
        Task SavePhoto(Photo photo);
        Task AssociateTags(Photo photo, params string[] tags);
        Task<Photo> GetPhotoByChecksum(string checksum);
        Task MergeTags(string newTagName, params int[] targetTagIds);
        Task CopyTags(string newTagName, int? sourceTagId);
        Task<IEnumerable<Tag>> GetTagsByKeywords(string keywords, int pageNum, int pageSize);
    }
}