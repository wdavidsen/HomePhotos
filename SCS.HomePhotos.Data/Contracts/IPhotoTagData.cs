using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The photo -> tag association data repository.
    /// </summary>
    public interface IPhotoTagData
    {
        /// <summary>
        /// Associates a photo to a tag.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A photo tag association.</returns>
        Task<PhotoTag> AssociatePhotoTag(int photoId, int tagId);

        /// <summary>
        /// Associates a photo to a tag.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="existingTagId">The existing tag identifier.</param>
        /// <param name="newTagId">The new tag identifier.</param>
        /// <returns>A photo tag association.</returns>
        Task<PhotoTag> AssociatePhotoTag(int photoId, int existingTagId, int newTagId);

        /// <summary>
        /// Dissociates a photo from a tag.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A void task.</returns>
        Task DissociatePhotoTag(int photoId, int tagId);

        /// <summary>
        /// Gets the photo associations for a tag.
        /// </summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo tag associations.</returns>
        Task<IEnumerable<UserPhotoTag>> GetPhotoTagAssociations(int tagId);

        /// <summary>
        /// Gets the photo tag associations for a photo and tag together.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="tagId">The tag identifier.</param>
        /// <returns>A list of photo tag associations.</returns>
        Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int photoId, int tagId);
    }
}