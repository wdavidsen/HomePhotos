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

            // "contains" match sql for individual words (when more than 1 is provided)
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
    }
}
