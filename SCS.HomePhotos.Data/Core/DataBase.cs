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
    /// <typeparam name="T"></typeparam>    
    public abstract class DataBase<T> : IDataBase<T> where T : class
    {
        private readonly IStaticConfig _staticConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBase{T}"/> class.
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

            var conn = new SqliteConnection($"Data Source={dbPath}");
            conn.Open();

            return conn;
        }

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity Id.</returns>
        public virtual async Task<int?> InsertAsync(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.InsertAsync(entity);
            }
        }

        /// <summary>
        /// Inserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity Id.</returns>
        public virtual int? Insert(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return connection.Insert(entity);
            }
        }

        /// <summary>
        /// Updates an entity.
        /// </summary>        
        /// <param name="entity">The entity.</param>
        /// <returns>The number of affected records.</returns>
        public virtual async Task<int?> UpdateAsync(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.UpdateAsync(entity);
            }
        }

        /// <summary>
        /// Gets an entity.
        /// </summary>        
        /// <param name="id">The entity id.</param>
        /// <returns>The matching entity.</returns>
        public virtual async Task<T> GetAsync(int id)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetAsync<T>(id);
            }
        }

        /// <summary>
        /// Gets a list of entities.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <returns>The list matching entities.</returns>
        public virtual async Task<IEnumerable<T>> GetListAsync(string whereClause, object parameters)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListAsync<T>(whereClause, parameters);
            }
        }

        /// <summary>
        /// Gets a list of all entities.
        /// </summary>        
        /// <returns>The list of all entities.</returns>
        public virtual async Task<IEnumerable<T>> GetListAsync()
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListAsync<T>();
            }
        }

        /// <summary>
        /// Gets a paged list of entities.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <param name="orderBy">The order by field.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>The list matching entities.</returns>
        public virtual async Task<IEnumerable<T>> GetListPagedAsync(string whereClause, object parameters, string orderBy, int pageNum, int pageSize)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListPagedAsync<T>(pageNum, pageSize, whereClause, orderBy, parameters);
            }
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>        
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The number of records affected.</returns>
        public virtual async Task<int?> DeleteAsync(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.DeleteAsync(entity);
            }
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>        
        /// <param name="id">The entity id to delete.</param>
        /// <returns>The number of records affected.</returns>
        public virtual async Task<int?> DeleteAsync(int id)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.DeleteAsync<T>(id);
            }
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>        
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameters">The where clause parameters.</param>
        /// <returns>The total record count.</returns>
        public virtual async Task<long> GetRecordCount(string whereClause, object parameters)
        {
            using (var connection = GetDbConnection())
            {
                var tableName = GetTableName();
                var sql = $"SELECT COUNT(*) FROM {tableName} {whereClause}";

                var result = await connection.ExecuteScalarAsync(sql, parameters);
                return (long)result;
            }
        }

        /// <summary>
        /// Gets the name of an entity's table.
        /// </summary>        
        /// <returns>The table name.</returns>
        public static string GetTableName()
        {
            var tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));
            return tableAttribute.Name;
        }
    }
}
