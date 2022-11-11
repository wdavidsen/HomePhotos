using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// Base class for data objects.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IDataBase" />
    public interface IDataBase
    {
        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity id to delete.</param>
        /// <returns>The number of records affected.</returns>
        Task<int?> DeleteAsync<T>(int id);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The number of records affected.</returns>
        Task<int?> DeleteAsync<T>(T entity);

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity id.</param>
        /// <returns>The matching entity.</returns>
        Task<T> GetAsync<T>(int id);

        /// <summary>
        /// Gets a list of all entities.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>        
        /// <returns>The list of all entities.</returns>
        Task<IEnumerable<T>> GetListAsync<T>();

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <returns>The list matching entities.</returns>
        Task<IEnumerable<T>> GetListAsync<T>(string whereClause, object parameters);

        /// <summary>
        /// Gets a paged list of entities.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <param name="orderBy">The order by field.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>The list matching entities.</returns>
        Task<IEnumerable<T>> GetListPagedAsync<T>(string whereClause, object parameters, string orderBy, int pageNum, int pageSize);

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity Id.</returns>
        Task<int?> InsertAsync<T>(T entity);

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity Id.</returns>
        int? Insert<T>(T entity);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The number of affected records.</returns>
        Task<int?> UpdateAsync<T>(T entity);

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <returns>The total record count.</returns>
        Task<long> GetRecordCount<T>(string whereClause, object parameters);
    }
}