using SCS.HomePhotos.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    public interface ITagData : IDataBase
    {
        Task<IEnumerable<Tag>> GetTags();

        Task<IEnumerable<TagStat>> GetTags(string keywords, int pageNum = 0, int pageSize = 200);

        Task<IEnumerable<TagStat>> GetTagAndPhotoCount();

        Task<TagStat> GetTagAndPhotoCount(string tagName);

        Task<Tag> SaveTag(Tag tag);

        Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int tagId);

        Task<IEnumerable<PhotoTag>> GetPhotoTagAssociations(int photoId, int tagId);

        Task<PhotoTag> AssociatePhotoTag(int photoId, int tagId);

        Task<PhotoTag> AssociatePhotoTag(int photoId, int existingTagId, int newTagId);
        Task DissociatePhotoTag(int photoId, int tagId);

        Task<Tag> GetTag(string tagName);
    }
}