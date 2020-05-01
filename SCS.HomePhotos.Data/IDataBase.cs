using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public interface IDataBase
    {
        Task<int?> DeleteAsync<T>(int id);
        Task<int?> DeleteAsync<T>(T entity);
        Task<T> GetAsync<T>(int id);
        Task<IEnumerable<T>> GetListAsync<T>();
        Task<IEnumerable<T>> GetListAsync<T>(string whereClause, object parameters);
        Task<IEnumerable<T>> GetListPagedAsync<T>(string whereClause, object parameters, string orderBy, int pageNum, int pageSize);
        Task<int?> InsertAsync<T>(T entity);
        int? Insert<T>(T entity);
        Task<int?> UpdateAsync<T>(T entity);
        Task<long> GetRecordCount<T>(string whereClause, object parameters);
    }
}