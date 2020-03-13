using System.Collections.Generic;
using System.Threading.Tasks;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Service
{
    public interface ITagData : IDataBase
    {
        Task<IEnumerable<Tag>> GetTags();

        Task<IEnumerable<TagStat>> GetTagAndPhotoCount();

        Task<Tag> SaveTag(Tag tag);
    }
}