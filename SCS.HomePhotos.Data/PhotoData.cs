using Dapper;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public class PhotoData : DataBase, IPhotoData
    {
        public PhotoData(IStaticConfig staticConfig) : base(staticConfig) { }

        public async Task<IEnumerable<Photo>> GetPhotos(string[] tags, int pageNum = 1, int pageSize = 200)
        {
            var offset = (pageNum - 1) * pageSize;

            var sql = $@"SELECT p.* 
                         FROM Photo p
                         JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         JOIN Tag t ON pt.TagId = t.TagId 
                         WHERE t.TagName IN @Tags
                         ORDER BY p.DateTaken DESC LIMIT {pageSize} OFFSET {offset}";

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<Photo>(sql, new { Tags = tags });
            }
        }

        public async Task<IEnumerable<Photo>> GetPhotos(DateTime dateTakenStart, DateTime dateTakenEnd, bool descending = true, int pageNum = 1, int pageSize = 200)
        {
            var where = "WHERE DateTaken >= @DateTakenStart AND DateTaken <= @DateTakenEnd";
            var parameters = new { DateTakenStart = dateTakenStart.ToStartOfDay(), DateTakenEnd = dateTakenEnd.ToEndOfDay() };
            var orderBy = "DateTaken" + (descending ? " DESC" : "");

            return await GetListPagedAsync<Photo>(where, parameters, orderBy, pageNum, pageSize);
        }

        public async Task<Photo> SavePhoto(Photo photo)
        {
            if (photo.PhotoId == null)
            {
                photo.PhotoId = await InsertAsync(photo);
            }
            else
            {
                await UpdateAsync(photo);
            }

            return photo;
        }

        public async Task<PhotoTag> AssociatePhotoTag(int photoId, int tagId)
        {
            var existingTag = await GetListAsync<PhotoTag>("WHERE PhotoId = @PhotoId AND TagId = @TagId", new { PhotoId = photoId, TagId = tagId });

            if (existingTag.Any())
            {
                return existingTag.First();
            }

            var photoTag = new PhotoTag
            {
                PhotoId = photoId,
                TagId = tagId
            };
            await InsertAsync(photoTag);
            return photoTag;
        }
    }
}
