using Dapper;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The tag repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Core.DataBase" />
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.ITagData" />
    public class TagData : DataBase, ITagData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public TagData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets all tags.
        /// </summary>
        /// <returns>A list of all tags.</returns>
        public async Task<IEnumerable<Tag>> GetTags()
        {
            return await GetListPagedAsync<Tag>("", new object(), "TagName ASC", 1, int.MaxValue);
        }

        /// <summary>
        /// Gets the tag by name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>The matching tag.</returns>
        public async Task<Tag> GetTag(string tagName)
        {
            var list = await GetListAsync<Tag>("WHERE TagName = @TagName", new { TagName = tagName });

            if (list.Count() > 0)
            {
                return list.First();
            }
            return null;
        }

        /// <summary>
        /// Gets the tag and photo count.
        /// </summary>
        /// <returns>A list of tags.</returns>
        public async Task<IEnumerable<TagStat>> GetTagAndPhotoCount()
        {
            var sql = @"SELECT t.TagId, t.TagName, COUNT(p.PhotoId) AS PhotoCount 
                        FROM Tag t LEFT JOIN PhotoTag pt ON t.TagId = pt.TagId LEFT JOIN Photo p ON pt.PhotoId = p.PhotoId 
                        GROUP BY t.TagName, t.TagId 
                        ORDER BY t.TagName";

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<TagStat>(sql);
            }
        }

        /// <summary>
        /// Gets the tag and photo count.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>The tag and photo count.</returns>
        public async Task<TagStat> GetTagAndPhotoCount(string tagName)
        {
            var sql = @"SELECT t.TagId, t.TagName, COUNT(p.PhotoId) AS PhotoCount 
                        FROM Tag t LEFT JOIN PhotoTag pt ON t.TagId = pt.TagId LEFT JOIN Photo p ON pt.PhotoId = p.PhotoId 
                        WHERE TagName = @TagName
                        GROUP BY t.TagName, t.TagId 
                        ORDER BY t.TagName";

            using (var conn = GetDbConnection())
            {
                return await conn.QuerySingleAsync<TagStat>(sql, new { TagName = tagName });
            }
        }

        /// <summary>
        /// Gets the tags by keyword.
        /// </summary>
        /// <param name="keywords">The search keywords.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of matching tags.</returns>
        public async Task<IEnumerable<TagStat>> GetTags(string keywords, int pageNum = 0, int pageSize = 200)
        {
            var offset = (pageNum - 1) * pageSize;
            var keywordArray = keywords.Split(' ').Select(kw => kw.Replace("'", "")).ToArray();
            var wordCount = keywordArray.Length;

            var _sql = $@"SELECT t.TagId, t.TagName, COUNT(p.PhotoId) AS PhotoCount, {{0}} as Weight  
                         FROM Photo p
                         LEFT JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         LEFT JOIN Tag t ON pt.TagId = t.TagId ";

            var _where1 = $"{Environment.NewLine}WHERE t.TagName <> @Tag{wordCount * 3 + 1} ";
            var _where2 = $"{Environment.NewLine}WHERE t.TagName <> '' ";

            var groupBy = $"{Environment.NewLine}GROUP BY t.TagName, t.TagId ";

            // "exact" match sql for individual words (when more than 1 is provided)
            var sql = string.Format(_sql, 2) + ((wordCount > 1) ? _where1 : _where2);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName = @Tag{j + 1}";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }
            sql += groupBy;

            // "starts with" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(_sql, 3) + ((wordCount > 1) ? _where1 : _where2);

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName <> @Tag{j + 1} AND t.TagName LIKE @Tag{wordCount + 1 + j}";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }
            sql += groupBy;

            // "contains" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(_sql, 4) + ((wordCount > 1) ? _where1 : _where2);

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
            if (wordCount > 1)
            {
                sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(_sql, 1) + _where2;
                sql += $"AND t.TagName = @Tag{dynamicCount + 1}";
                dynamicParams.Add($"@Tag{dynamicCount + 1}", keywords);
                sql += groupBy;

                sql += $"{Environment.NewLine}ORDER BY Weight ASC, TagName ASC LIMIT {pageSize} OFFSET {offset}";
            }

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<TagStat>(sql, dynamicParams);
            }
        }

        /// <summary>
        /// Saves a tag.
        /// </summary>
        /// <param name="tag">The tag to save.</param>
        /// <returns>The saved tag.</returns>
        public async Task<Tag> SaveTag(Tag tag)
        {
            if (tag.TagId == null || tag.TagId == 0)
            {
                tag.TagId = await InsertAsync(tag);
            }
            else
            {
                var existingTag = await GetAsync<Tag>(tag.TagId.Value);

                if (existingTag == null)
                {
                    throw new InvalidOperationException($"Tag id {tag.TagId} was not found.");
                }

                await UpdateAsync(tag);
            }

            return tag;
        }

        /// <summary>
        /// Gets the photos-tag associations for a tag id.
        /// </summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo-tags.</returns>
        public async Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int tagId)
        {
            return await GetListAsync<PhotoTag>("WHERE TagId = @TagId", new { TagId = tagId });
        }

        /// <summary>
        /// Gets the photos-tag associations for a photo id and tag id.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo-tags.</returns>
        public async Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int photoId, int tagId)
        {
            return await GetListAsync<PhotoTag>("WHERE PhotoId = @PhotoId AND TagId = @TagId", new { PhotoId = photoId, TagId = tagId });
        }

        /// <summary>
        /// Associates a photo with a tag.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A new photo-tag entity.</returns>
        public async Task<PhotoTag> AssociatePhotoTag(int photoId, int tagId)
        {
            var existingTags = await GetPhotoTagAssociations(photoId, tagId);

            if (existingTags.Any())
            {
                return existingTags.First();
            }

            var photoTag = new PhotoTag
            {
                PhotoId = photoId,
                TagId = tagId
            };
            await InsertAsync(photoTag);
            return photoTag;
        }

        /// <summary>
        /// Changes a photo tag association.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="existingTagId">The existing tag identifier.</param>
        /// <param name="newTagId">The new tag identifier.</param>
        /// <returns>A new photo-tag entity.</returns>
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
        /// <summary>
        /// Dissociates a photo from a tag.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A void task.</returns>
        public async Task DissociatePhotoTag(int photoId, int tagId)
        {
            var existingTags = await GetPhotoTagAssociations(photoId, tagId);

            foreach (var photoTag in existingTags)
            {
                await DeleteAsync(photoTag);
            }
        }

        /// <summary>
        /// Builds the search parameter values.
        /// </summary>
        /// <param name="keywordArray">The keyword array.</param>
        /// <returns>The Dapper dynamic parameters.</returns>
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
