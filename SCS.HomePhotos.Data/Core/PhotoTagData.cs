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
    public class PhotoTagData : DataBase<PhotoTag>, IPhotoTagData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoTagData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public PhotoTagData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets the photos-tag associations for a tag id.
        /// </summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo-tags.</returns>
        public async Task<IEnumerable<UserPhotoTag>> GetPhotoTagAssociations(int tagId)
        {
            var sql = @"SELECT pt.PhotoTagId, pt.PhotoId, t.TagId, t.TagName, t.UserId  
                        FROM Tag t 
                        JOIN PhotoTag pt ON t.TagId = pt.TagId 
                        WHERE t.TagId = @TagId 
                        ORDER BY t.TagName";

            using (var conn = GetDbConnection())
            {
                return await conn.QueryAsync<UserPhotoTag>(sql, new { TagId = tagId });
            }
        }

        /// <summary>
        /// Gets the photos-tag associations for a photo id and tag id.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo-tags.</returns>
        public async Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int photoId, int tagId)
        {
            return await GetListAsync("WHERE PhotoId = @PhotoId AND TagId = @TagId", new { PhotoId = photoId, TagId = tagId });
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
            var existingTag = await GetListAsync("WHERE PhotoId = @PhotoId AND TagId = @TagId", new { PhotoId = photoId, TagId = existingTagId });

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
    }
}
