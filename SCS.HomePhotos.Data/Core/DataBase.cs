using Dapper;
using Microsoft.Data.Sqlite;
using SCS.HomePhotos.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// Base class for data objects.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IDataBase" />
    public abstract class DataBase : IDataBase
    {
        private readonly IStaticConfig _staticConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBase"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public DataBase(IStaticConfig staticConfig)
        {
            _staticConfig = staticConfig;
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <returns>A database connection</returns>
        protected DbConnection GetDbConnection()
        {
            var dbPath = _staticConfig.DatabasePath;

            if (!Path.IsPathRooted(dbPath))
            {
                dbPath = AppDomain.CurrentDomain.BaseDirectory + dbPath;
            }

            return new SqliteConnection($"Data Source={dbPath}");
        }

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity Id.</returns>
        public virtual async Task<int?> InsertAsync<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.InsertAsync(entity);
            }
        }

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity Id.</returns>
        public virtual int? Insert<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return connection.Insert(entity);
            }
        }

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The number of affected records.</returns>
        public virtual async Task<int?> UpdateAsync<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.UpdateAsync(entity);
            }
        }

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity id.</param>
        /// <returns>The matching entity.</returns>
        public virtual async Task<T> GetAsync<T>(int id)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetAsync<T>(id);
            }
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <returns>The list matching entities.</returns>
        public virtual async Task<IEnumerable<T>> GetListAsync<T>(string whereClause, object parameters)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListAsync<T>(whereClause, parameters);
            }
        }

        /// <summary>
        /// Gets a list of all entities.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>        
        /// <returns>The list of all entities.</returns>
        public virtual async Task<IEnumerable<T>> GetListAsync<T>()
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListAsync<T>();
            }
        }

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
        public virtual async Task<IEnumerable<T>> GetListPagedAsync<T>(string whereClause, object parameters, string orderBy, int pageNum, int pageSize)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListPagedAsync<T>(pageNum, pageSize, whereClause, orderBy, parameters);
            }
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The number of records affected.</returns>
        public virtual async Task<int?> DeleteAsync<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.DeleteAsync(entity);
            }
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity id to delete.</param>
        /// <returns>The number of records affected.</returns>
        public virtual async Task<int?> DeleteAsync<T>(int id)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.DeleteAsync<T>(id);
            }
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <returns>The total record count.</returns>
        public virtual async Task<long> GetRecordCount<T>(string whereClause, object parameters)
        {
            using (var connection = GetDbConnection())
            {
                var tableName = GetTableName<T>();
                var sql = $"SELECT COUNT(*) FROM {tableName} {whereClause}";

                var result = await connection.ExecuteScalarAsync(sql, parameters);
                return (long)result;
            }
        }

        /// <summary>
        /// Gets the name of an entity's table.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The table name.</returns>
        public static string GetTableName<T>()
        {
            var tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));
            return tableAttribute.Name;
        }
    }
}
