using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    public interface IPhotoTagData
    {
        Task<PhotoTag> AssociatePhotoTag(int photoId, int tagId);

        Task<PhotoTag> AssociatePhotoTag(int photoId, int existingTagId, int newTagId);

        Task DissociatePhotoTag(int photoId, int tagId);

        Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int tagId);

        Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int photoId, int tagId);
    }
}