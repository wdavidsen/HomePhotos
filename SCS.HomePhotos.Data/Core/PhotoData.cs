using Dapper;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
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

        public async Task<IEnumerable<Photo>> GetPhotos(string keywords, int pageNum = 0, int pageSize = 200)
        {
            var offset = (pageNum - 1) * pageSize;
            var keywordArray = keywords.Split(' ').Select(kw => kw.Replace("'", "")).ToArray();
            var wordCount = keywordArray.Length;

            var _sql = $@"SELECT p.*, {{0}} as Weight  
                         FROM Photo p
                         JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         JOIN Tag t ON pt.TagId = t.TagId ";

            var _where1 = $"{Environment.NewLine}WHERE t.TagName <> @Tag{wordCount * 3 + 1} ";
            var _where2 = $"{Environment.NewLine}WHERE t.TagName <> '' ";

            // "exact" match sql for individual words (when more than 1 is provided)
            var sql = string.Format(_sql, 2) + ((wordCount > 1) ? _where1 : _where2);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName = @Tag{j + 1} ";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }

            // "starts with" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION{Environment.NewLine}" + string.Format(_sql, 3) + ((wordCount > 1) ? _where1 : _where2);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName <> @Tag{j + 1} AND t.TagName LIKE @Tag{wordCount + 1 + j} ";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }

            // "contains" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION{Environment.NewLine}" + string.Format(_sql, 4) + ((wordCount > 1) ? _where1 : _where2);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName <> @Tag{j + 1} AND t.TagName NOT LIKE @Tag{wordCount + 1 + j} AND t.TagName LIKE @Tag{(2 * wordCount) + 1 + j} ";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }

            var dynamicParams = BuildSearchParameterValues(keywordArray);
            var dynamicCount = dynamicParams.ParameterNames.Count();

            // "exact" match of full keyword phrase
            if (wordCount > 1)
            {
                sql += $"{Environment.NewLine}UNION{Environment.NewLine}" + string.Format(_sql, 1) + _where2;
                sql += $"AND t.TagName = @Tag{dynamicCount + 1}";
                
                dynamicParams.Add($"@Tag{dynamicCount + 1}", keywords);

                sql += $"{Environment.NewLine}ORDER BY Weight ASC, DateTaken DESC LIMIT {pageSize} OFFSET {offset}";
            }

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<Photo>(sql, dynamicParams);
            }
        }

        public async Task<IEnumerable<Tag>> GetTagsAndPhotos(int[] photoIds)
        {
            var sql = @"SELECT t.TagId, t.TagName, p.PhotoId, p.Checksum, p.Name, p.Name, p.FileName, p.DateTaken, p.DateFileCreated, p.CacheFolder, p.ImageHeight, p.ImageWidth 
                        FROM Photo p 
                        JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId 
                        JOIN Tag t ON pt.TagId = t.TagId 
                        WHERE p.PhotoId IN @PhotoIds ";

            var lookup = new Dictionary<int, Tag>();

            using (var conn = GetDbConnection())
            {
                var tags = await conn.QueryAsync<Tag, Photo, Tag>(sql, (tag, photo) =>
                {
                    var isDup = true;

                    if (!lookup.TryGetValue(tag.TagId.Value, out var tempTag))
                    {
                        isDup = false;
                        lookup.Add(tag.TagId.Value, tempTag = tag);
                    }

                    tempTag.Photos = tempTag.Photos ?? new List<Photo>();
                    tempTag.Photos.Add(photo);

                    return isDup ? null : tempTag;
                },
                splitOn: "PhotoId", param: new { PhotoIds = photoIds });

                return tags.Where(t => t != null);
            }
        }

        public async Task<IEnumerable<Photo>> GetPhotosAndTags(int[] photoIds)
        {
            var sql = @"SELECT p.PhotoId, p.Checksum, p.Name, p.Name, p.FileName, p.DateTaken, p.DateFileCreated, p.CacheFolder, p.ImageHeight, p.ImageWidth, t.TagId, t.TagName   
                        FROM Photo p 
                        JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId 
                        JOIN Tag t ON pt.TagId = t.TagId 
                        WHERE p.PhotoId IN @PhotoIds ";

            var lookup = new Dictionary<int, Photo>();

            using (var conn = GetDbConnection())
            {
                var tags = await conn.QueryAsync<Photo, Tag, Photo>(sql, (photo, tag) =>
                {
                    var isDup = true;

                    if (!lookup.TryGetValue(photo.PhotoId.Value, out var tempPhoto))
                    {
                        isDup = false;
                        lookup.Add(photo.PhotoId.Value, tempPhoto = photo);
                    }

                    tempPhoto.Tags = tempPhoto.Tags ?? new List<Tag>();
                    tempPhoto.Tags.Add(tag);

                    return isDup ? null : tempPhoto;
                },
                splitOn: "TagId", param: new { PhotoIds = photoIds });

                return tags.Where(t => t != null);
            }
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
        
        private static DynamicParameters BuildSearchParameterValues(string[] keywordArray)
        {
            var values = new Dictionary<string, object>();
            var ctr = 1;

            for (var i = 0; i < 3; i++)
            {
                var valueTemplate = (i == 0) ? "{0}" : ((i == 1) ? "%{0}" : "%{0}%");

                for (var j = 0; j < keywordArray.Length; j++)
                {
                    var keyword = keywordArray[j];
                    values.Add($"@Tag{ctr}", string.Format(valueTemplate, keyword));
                    ctr++;
                }
            }

            return new DynamicParameters(values);
        }

        public async Task FlagPhotosForReprocessing()
        {
            var sql = "UPDATE Photo SET ReprocessCache = 1";

            using (var conn = GetDbConnection())
            {
                await conn.ExecuteScalarAsync(sql);
            }
        }

        public async Task DeletePhotos()
        {
            var sql1 = "DELETE FROM PhotoTag";
            var sql2 = "DELETE FROM Photo";

            using (var conn = GetDbConnection())
            {
                await conn.ExecuteScalarAsync(sql1);
                await conn.ExecuteScalarAsync(sql2);
            }
        }
    }
}
