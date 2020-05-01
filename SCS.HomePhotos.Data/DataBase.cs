using Dapper;
using Microsoft.Data.Sqlite;
using SCS.HomePhotos.Service;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public abstract class DataBase : IDataBase
    {
        private readonly IStaticConfig _staticConfig;

        public DataBase(IStaticConfig staticConfig)
        {
            _staticConfig = staticConfig;
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
        }

        protected DbConnection GetDbConnection()
        {
            var dbPath = _staticConfig.DatabasePath;

            if (!Path.IsPathRooted(dbPath))
            {
                dbPath = AppDomain.CurrentDomain.BaseDirectory + dbPath;
            }

            return new SqliteConnection($"Data Source={dbPath}");
        }

        public virtual async Task<int?> InsertAsync<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.InsertAsync(entity);
            }
        }

        public virtual int? Insert<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return connection.Insert(entity);
            }
        }

        public virtual async Task<int?> UpdateAsync<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.UpdateAsync(entity);
            }
        }

        public virtual async Task<T> GetAsync<T>(int id)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetAsync<T>(id);
            }
        }

        public virtual async Task<IEnumerable<T>> GetListAsync<T>(string whereClause, object parameters)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListAsync<T>(whereClause, parameters);
            }
        }

        public virtual async Task<IEnumerable<T>> GetListAsync<T>()
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListAsync<T>();
            }
        }

        public virtual async Task<IEnumerable<T>> GetListPagedAsync<T>(string whereClause, object parameters, string orderBy, int pageNum, int pageSize)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.GetListPagedAsync<T>(pageNum, pageSize, whereClause, orderBy, parameters);
            }
        }

        public virtual async Task<int?> DeleteAsync<T>(T entity)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.DeleteAsync(entity);
            }
        }

        public virtual async Task<int?> DeleteAsync<T>(int id)
        {
            using (var connection = GetDbConnection())
            {
                return await connection.DeleteAsync<T>(id);
            }
        }

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

        public static string GetTableName<T>()
        {
            var tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));
            return tableAttribute.Name;
        }
    }
}
