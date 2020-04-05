using Dapper;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public class TagData : DataBase, ITagData
    {
        public TagData(IStaticConfig staticConfig) : base(staticConfig) { }

        public async Task<IEnumerable<Tag>> GetTags()
        {
            return await GetListPagedAsync<Tag>("", new object(), "TagName ASC", 1, int.MaxValue);
        }

        public async Task<IEnumerable<TagStat>> GetTagAndPhotoCount()
        {
            var sql = @"SELECT t.TagId, t.TagName, COUNT(p.PhotoId) AS PhotoCount 
                        FROM Tag t JOIN PhotoTag pt ON t.TagId = pt.TagId JOIN Photo p ON pt.PhotoId = p.PhotoId 
                        GROUP BY t.TagName, t.TagId 
                        ORDER BY t.TagName";

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<TagStat>(sql);
            }
        }

        public async Task<IEnumerable<TagStat>> GetTags(string keywords, int pageNum = 0, int pageSize = 200)
        {
            var offset = (pageNum - 1) * pageSize;
            var keywordArray = keywords.Split(' ').Select(kw => kw.Replace("'", "")).ToArray();
            var wordCount = keywordArray.Length;

            var _sql = $@"SELECT t.TagId, t.TagName, COUNT(p.PhotoId) AS PhotoCount, {{0}} as Weight  
                         FROM Photo p
                         JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         JOIN Tag t ON pt.TagId = t.TagId 
                         WHERE t.TagName <> @Tag{wordCount * 3 + 1} ";

            var groupBy = $"{Environment.NewLine}GROUP BY t.TagName, t.TagId ";

            // "exact" match sql for individual words (when more than 1 is provided)
            var sql = string.Format(_sql, 2);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName = @Tag{j + 1}";
                sql += (j == keywordArray.Length - 1) ? ") " :  " ";
            }
            sql += groupBy;

            // "starts with" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(_sql, 3);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName <> @Tag{j + 1} AND t.TagName LIKE @Tag{wordCount + 1 + j}";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }
            sql += groupBy;

            // "contains" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(_sql, 4);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName <> @Tag{j + 1} AND t.TagName NOT LIKE @Tag{wordCount + 1 + j} AND t.TagName LIKE @Tag{(2 * wordCount) + 1 + j}";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }
            sql += groupBy;

            var dynamicParams = BuildSearchParameterValues(keywordArray);
            var dynamicCount = dynamicParams.ParameterNames.Count();

            // "exact" match of full keyword phrase
            sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(_sql, 1);
            sql += $"AND t.TagName = @Tag{dynamicCount + 1}";
            dynamicParams.Add($"@Tag{dynamicCount + 1}", keywords);
            sql += groupBy;

            sql += $"{Environment.NewLine}ORDER BY Weight ASC, TagName ASC LIMIT {pageSize} OFFSET {offset}";

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<TagStat>(sql, dynamicParams);
            }
        }

        public async Task<Tag> SaveTag(Tag tag)
        {
            if (tag.TagId == null)
            {
                tag.TagId = await InsertAsync(tag);
            }
            else
            {
                await UpdateAsync(tag);
            }

            return tag;
        }

        public async Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int tagId)
        {
            return await GetListAsync<PhotoTag>("WHERE TagId = @TagId", new { TagId = tagId });
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

        public async Task<PhotoTag> AssociatePhotoTag(int photoId, int existingTagId, int newTagId)
        {
            var existingTag = await GetListAsync<PhotoTag>("WHERE PhotoId = @PhotoId AND TagId = @TagId", new { PhotoId = photoId, TagId = existingTagId });

            if (!existingTag.Any())
            {
                throw new InvalidOperationException($"{nameof(existingTagId)} not found.");
            }

            var photoTag = existingTag.First();
            photoTag.TagId = newTagId;
            await UpdateAsync(photoTag);

            return photoTag;
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
