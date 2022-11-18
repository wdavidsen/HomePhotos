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
    public class TagData : DataBase<Tag>, ITagData
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
            return await GetListPagedAsync("", new object(), "TagName ASC", 1, int.MaxValue);
        }

        /// <summary>
        /// Gets the tag by name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="userId">The owner of the tag.</param>
        /// <returns>The matching tag.</returns>
        public async Task<Tag> GetTag(string tagName, int? userId = null)
        {
            IEnumerable<Tag> list;

            if (userId == null)
            {
                list = await GetListAsync("WHERE TagName = @TagName AND UserId IS NULL", new { TagName = tagName });
            }
            else
            {
                list = await GetListAsync("WHERE TagName = @TagName AND UserId = @UserId", new { TagName = tagName, UserId = userId });
            }

            if (list.Any())
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
            var sql = @"SELECT t.TagId, t.TagName, t.UserId, u.TagColor, COUNT(p.PhotoId) AS PhotoCount 
                        FROM Tag t 
                        LEFT JOIN PhotoTag pt ON t.TagId = pt.TagId 
                        LEFT JOIN Photo p ON pt.PhotoId = p.PhotoId 
                        LEFT JOIN User u ON t.UserId = u.UserId 
                        GROUP BY t.TagName, t.TagId, t.UserId, u.TagColor   
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
        /// <param name="userId">Name user id owner.</param>
        /// <returns>The tag and photo count.</returns>
        public async Task<TagStat> GetTagAndPhotoCount(string tagName, int? userId)
        {
            var sql = @"SELECT t.TagId, t.TagName, t.UserId, u.TagColor, COUNT(p.PhotoId) AS PhotoCount 
                        FROM Tag t 
                        LEFT JOIN PhotoTag pt ON t.TagId = pt.TagId 
                        LEFT JOIN Photo p ON pt.PhotoId = p.PhotoId 
                        LEFT JOIN User u ON t.UserId = u.UserId 
                        WHERE TagName = @TagName AND u.UserId = @UserId 
                        GROUP BY t.TagName, t.TagId, t.UserId, u.TagColor   
                        ORDER BY t.TagName";

            using (var conn = GetDbConnection())
            {
                return await conn.QuerySingleAsync<TagStat>(sql, new { TagName = tagName, UserId = userId });
            }
        }

        /// <summary>
        /// Gets the tags by keyword.
        /// </summary>
        /// <param name="keywords">The search keywords.</param>
        /// <param name="dateRange">The optional date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of matching tags.</returns>
        public async Task<IEnumerable<TagStat>> GetTags(string keywords, DateRange? dateRange = null, int pageNum = 0, int pageSize = 200)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new ArgumentException("Keyword cannot be null or empty.", nameof(keywords));
            }

            var offset = (pageNum - 1) * pageSize;
            var keywordArray = keywords.Split(' ').Select(kw => kw.Replace("'", "")).ToArray();
            var wordCount = keywordArray.Length;

            var mainSql = $@"SELECT t.TagId, t.TagName, t.UserId, u.TagColor, COUNT(p.PhotoId) AS PhotoCount, {{0}} as Weight  
                         FROM Photo p
                         LEFT JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         LEFT JOIN Tag t ON pt.TagId = t.TagId 
                         LEFT JOIN User u ON t.UserId = u.UserId ";

            var where1 = $"{Environment.NewLine}WHERE t.TagName <> @Tag{wordCount * 3 + 1} ";
            var where2 = $"{Environment.NewLine}WHERE t.TagName <> '' ";
            var where3 = dateRange != null ? $"AND p.DateTaken BETWEEN @FromDate AND @ToDate " : string.Empty;

            var groupBy = $"{Environment.NewLine}GROUP BY t.TagName, t.TagId, t.UserId, u.TagColor ";

            // "exact" match sql for individual words (when more than 1 is provided)
            var sql = string.Format(mainSql, 2) + ((wordCount > 1) ? where1 : where2) + where3;

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName = @Tag{j + 1}";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }
            sql += groupBy;

            // "starts with" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(mainSql, 3) + ((wordCount > 1) ? where1 : where2) + where3;

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName <> @Tag{j + 1} AND t.TagName LIKE @Tag{wordCount + 1 + j}";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }
            sql += groupBy;

            // "contains" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(mainSql, 4) + ((wordCount > 1) ? where1 : where2) + where3;

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
                sql += $"{Environment.NewLine}UNION ALL{Environment.NewLine}" + string.Format(mainSql, 1) + where2 + where3;
                sql += $"AND t.TagName = @Tag{dynamicCount + 1}";
                dynamicParams.Add($"@Tag{dynamicCount + 1}", keywords);
                sql += groupBy;

                sql += $"{Environment.NewLine}ORDER BY Weight ASC, TagName ASC LIMIT {pageSize} OFFSET {offset}";
            }

            // date taken range if specified
            if (where3.Length > 0)
            {
                var range = dateRange.Value;

                if (range.FromDate > range.ToDate)
                {
                    (range.ToDate, range.FromDate) = (range.FromDate, range.ToDate);
                }
                dynamicParams.Add("@FromDate", range.FromDate.ToStartOfDay().ToString(Constants.DatabaseDateTimeFormat));
                dynamicParams.Add("@ToDate", range.ToDate.ToEndOfDay().ToString(Constants.DatabaseDateTimeFormat));
            }

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<TagStat>(sql, dynamicParams);
            }
        }

        /// <summary>
        /// Gets the tags by keyword.
        /// </summary>
        /// <param name="dateRange">The date range.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of matching tags.</returns>
        public async Task<IEnumerable<TagStat>> GetTags(DateRange dateRange, int pageNum = 0, int pageSize = 200)
        {
            if (dateRange.FromDate > dateRange.ToDate)
            {
                (dateRange.ToDate, dateRange.FromDate) = (dateRange.FromDate, dateRange.ToDate);
            }

            var offset = (pageNum - 1) * pageSize;

            var sql = $@"SELECT t.TagId, t.TagName, t.UserId, u.TagColor, COUNT(p.PhotoId) AS PhotoCount   
                         FROM Photo p
                         JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         JOIN Tag t ON pt.TagId = t.TagId 
                         LEFT JOIN User u ON t.UserId = u.UserId 
                         WHERE p.DateTaken BETWEEN @FromDate AND @ToDate 
                         GROUP BY t.TagId, t.TagName, t.UserId, u.TagColor  
                         ORDER BY p.DateTaken DESC LIMIT {pageSize} OFFSET {offset} ";

            var fromDate = dateRange.FromDate.ToStartOfDay().ToString(Constants.DatabaseDateTimeFormat);
            var toDate = dateRange.ToDate.ToEndOfDay().ToString(Constants.DatabaseDateTimeFormat);

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<TagStat>(sql, new { FromDate = fromDate, ToDate = toDate });
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
                var existingTag = await GetAsync(tag.TagId.Value);

                if (existingTag == null)
                {
                    throw new InvalidOperationException($"Tag id {tag.TagId} was not found.");
                }

                await UpdateAsync(tag);
            }

            return tag;
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
