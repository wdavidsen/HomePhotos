﻿using Dapper;

using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The photo repository.
    /// </summary>    
    public class PhotoData : DataBase<Photo>, IPhotoData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public PhotoData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets a list of photos by tag.
        /// </summary>
        /// <param name="userFilter">The user filter information.</param>
        /// <param name="tag">The tag to search on.</param>
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        public async Task<IEnumerable<Photo>> GetPhotos(UserFilter userFilter, string tag, int pageNum = 1, int pageSize = 200)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException("Tag cannot be null or empty.", nameof(tag));
            }

            var offset = (pageNum - 1) * pageSize;
            var userClause = userFilter.GetUserScopeWhereClause("p", "t", true);

            var sql = $@"SELECT DISTINCT p.* 
                         FROM Photo p
                         JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         JOIN Tag t ON pt.TagId = t.TagId 
                         WHERE t.TagName = @Tag {userClause.Sql}    
                         ORDER BY p.DateTaken DESC LIMIT {pageSize} OFFSET {offset}";

            var parameters = new DynamicParameters();
            parameters.Add("Tag", tag);
            parameters.AddDynamicParams(userClause.Parameters);

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<Photo>(sql, parameters);
            }
        }

        /// <summary>
        /// Gets a list of photos matching search criteria.
        /// </summary>        
        /// <param name="userFilter">The user filter information.</param>
        /// <param name="dateRange">The date taken range.</param>        
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        public async Task<IEnumerable<Photo>> GetPhotos(UserFilter userFilter, DateRange dateRange, int pageNum = 1, int pageSize = 200)
        {
            var offset = (pageNum - 1) * pageSize;
            var dateClause = dateRange.GetWhereClause("p", false);
            var userClause = userFilter.GetUserScopeWhereClause("p", "t", true);

            var sql = $@"SELECT DISTINCT p.* 
                         FROM Photo p
                         JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         JOIN Tag t ON pt.TagId = t.TagId 
                         WHERE {dateClause.Sql} {userClause.Sql}    
                         ORDER BY p.DateTaken DESC LIMIT {pageSize} OFFSET {offset}";

            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(dateClause.Parameters);
            parameters.AddDynamicParams(userClause.Parameters);

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<Photo>(sql, parameters);
            }
        }

        /// <summary>
        /// Gets a list of photos by keywords.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="userFilter">The user filter information.</param>
        /// <param name="dateRange">The optional date range.</param>
        /// <param name="pageNum">The list page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A photo page list.</returns>
        public async Task<IEnumerable<Photo>> GetPhotos(UserFilter userFilter, DateRange dateRange, string keywords, int pageNum = 0, int pageSize = 200)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new ArgumentException("Keywords cannot be null or empty.", nameof(keywords));
            }

            var offset = (pageNum - 1) * pageSize;
            var keywordArray = keywords.Split(' ').Select(kw => kw.Replace("'", "")).ToArray();
            var wordCount = keywordArray.Length;

            var mainsql = $@"SELECT DISTINCT p.*, {{0}} as Weight  
                         FROM Photo p
                         JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId
                         JOIN Tag t ON pt.TagId = t.TagId ";

            var where1 = $"{Environment.NewLine}WHERE t.TagName <> @Tag{wordCount * 3 + 1} ";
            var where2 = $"{Environment.NewLine}WHERE t.TagName <> '' ";
            var dateClause = dateRange.GetWhereClause("p", true);
            var userClause = userFilter.GetUserScopeWhereClause("p", "t", true);

            // "exact" match sql for individual words (when more than 1 is provided)
            var sql = string.Format(mainsql, 2) + ((wordCount > 1) ? where1 : where2) + dateClause.Sql + userClause.Sql;

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName = @Tag{j + 1} ";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }

            // "starts with" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION{Environment.NewLine}" + string.Format(mainsql, 3) + ((wordCount > 1) ? where1 : where2) + dateClause.Sql + userClause.Sql;

            for (var j = 0; j < keywordArray.Length; j++)
            {
                sql += (j == 0) ? "AND (" : "OR ";
                sql += $"t.TagName <> @Tag{j + 1} AND t.TagName LIKE @Tag{wordCount + 1 + j} ";
                sql += (j == keywordArray.Length - 1) ? ") " : " ";
            }

            // "contains" match sql for individual words (when more than 1 is provided)
            sql += $"{Environment.NewLine}UNION{Environment.NewLine}" + string.Format(mainsql, 4) + ((wordCount > 1) ? where1 : where2) + dateClause.Sql + userClause.Sql;

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
                sql += $"{Environment.NewLine}UNION{Environment.NewLine}" + string.Format(mainsql, 1) + where2 + dateClause.Sql + userClause.Sql;
                sql += $"AND t.TagName = @Tag{dynamicCount + 1}";

                dynamicParams.Add($"@Tag{dynamicCount + 1}", keywords);
            }

            // apply weighting and paging
            if (wordCount > 1)
            {
                //sql += $"{Environment.NewLine}ORDER BY Weight ASC, DateTaken DESC LIMIT {pageSize} OFFSET {offset}";
                sql += $"{Environment.NewLine}ORDER BY Weight ASC, DateTaken DESC";
            }

            // date taken range if specified
            if (dateClause.Sql.Length > 0)
            {
                dynamicParams.AddDynamicParams(dateClause.Parameters);
            }

            if (userClause.Sql.Length > 0)
            {
                dynamicParams.AddDynamicParams(userClause.Parameters);
            }

            IEnumerable<Photo> photos;

            using (var conn = GetDbConnection())
            {
                photos = await conn.QueryAsync<Photo>(sql, dynamicParams);                
            }

            var distinctPhotos = photos.Distinct().ToList();
            var start = (pageNum - 1) * pageSize;

            return distinctPhotos.Skip(start).Take(pageSize);
        }

        /// <summary>
        /// Gets a list of photos by photo ids.
        /// </summary>        
        /// <param name="photoIds">The photo ids.</param>
        /// <param name="userId">The owner username of the tags.</param>
        /// <returns>A photo page list.</returns>
        public async Task<IEnumerable<Tag>> GetTagsAndPhotos(int[] photoIds, int? userId = null)
        {
            var userClause = userId == null ? "t.UserId IS NULL " : "t.UserId = @UserId ";

            var sql = @$"SELECT t.TagId, t.TagName, p.PhotoId, p.Checksum, p.Name, p.Name, p.FileName, p.DateTaken, p.DateFileCreated, p.CacheFolder, p.ImageHeight, p.ImageWidth 
                        FROM Photo p 
                        JOIN PhotoTag pt ON p.PhotoId = pt.PhotoId 
                        JOIN Tag t ON pt.TagId = t.TagId 
                        WHERE p.PhotoId IN @PhotoIds AND {userClause} 
                        ORDER BY t.TagName ";

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

                    tempTag.Photos ??= new List<Photo>();
                    tempTag.Photos.Add(photo);

                    return isDup ? null : tempTag;
                },
                splitOn: "PhotoId", param: new { PhotoIds = photoIds, UserId = userId });

                return tags.Where(t => t != null);
            }
        }

        /// <summary>
        /// Gets a list of photos and tags by photo ids.
        /// </summary>
        /// <param name="username">The owner username of the tags.</param>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>A photo page list.</returns>
        public async Task<IEnumerable<Photo>> GetPhotosAndTags(string username, int[] photoIds)
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

                    tempPhoto.Tags ??= new List<Tag>();
                    tempPhoto.Tags.Add(tag);

                    return isDup ? null : tempPhoto;
                },
                splitOn: "TagId", param: new { PhotoIds = photoIds });

                return tags.Where(t => t != null);
            }
        }

        /// <summary>
        /// Saves a photo entity.
        /// </summary>
        /// <param name="photo">The photo entity.</param>
        /// <returns>The saved photo entity.</returns>
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

        /// <summary>
        /// Flags all photos for reprocessing.
        /// </summary>
        /// <returns>A void task.</returns>
        public async Task FlagPhotosForReprocessing()
        {
            var sql = "UPDATE Photo SET ReprocessCache = 1";

            using (var conn = GetDbConnection())
            {
                await conn.ExecuteScalarAsync(sql);
            }
        }

        /// <summary>
        /// Deletes all photos.
        /// </summary>
        /// <returns>A void task.</returns>
        public async Task DeletePhotos()
        {
            var sql1 = "DELETE FROM PhotoTag";
            var sql2 = "DELETE FROM Photo";
            var sql3 = "DELETE FROM Tag";

            using (var conn = GetDbConnection())            
            {
                await conn.ExecuteScalarAsync(sql1);
                await conn.ExecuteScalarAsync(sql2);
                await conn.ExecuteScalarAsync(sql3);
            }
        }

        /// <summary>
        /// Deletes a photo by its original index location.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> photo was a mobile upload.</param>
        /// <param name="originalFolder">The original folder path relative to the index/mobile upload folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A void task.</returns>
        public async Task DeletePhoto(bool mobileUpload, string originalFolder, string fileName)
        {
            var sql = "DELETE FROM PhotoTag WHERE MobileUpload = @MobileUpload AND OriginalFolder = @OriginalFolder AND FileName = @FileName";
            
            using (var conn = GetDbConnection())            
            {
                await conn.ExecuteScalarAsync(sql, new { MobileUpload = mobileUpload, OriginalFolder  = originalFolder, FileName = fileName });
            }
        }

        /// <summary>
        /// Deletes all photos under a specific directory.
        /// </summary>
        /// <param name="mobileUpload">if set to <c>true</c> directory is under mobile uploads folder.</param>
        /// <param name="originalFolder">The original folder relative to the index/mobile uploads folder.</param>
        public async Task DeleteDirectoryPhotos(bool mobileUpload, string originalFolder)
        {
            originalFolder = originalFolder.Trim('/', '\\');

            var sql1 = "DELETE FROM PhotoTag WHERE PhotoId IN (SELECT PhotoId FROM Photo WHERE MobileUpload = @MobileUpload AND OriginalFolder = @OriginalFolder)";
            var sql2 = "DELETE FROM Photo WHERE MobileUpload = @MobileUpload AND OriginalFolder = @OriginalFolder";

            using (var conn = GetDbConnection())
            using (var trans = await conn.BeginTransactionAsync())
            {
                await conn.ExecuteScalarAsync(sql1, new { MobileUpload = mobileUpload, OriginalFolder = originalFolder });
                await conn.ExecuteScalarAsync(sql2, new { MobileUpload = mobileUpload, OriginalFolder = originalFolder });
                trans.Commit();             
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
