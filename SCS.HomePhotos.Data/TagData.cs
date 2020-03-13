using Dapper;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using System.Collections.Generic;
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
    }
}
