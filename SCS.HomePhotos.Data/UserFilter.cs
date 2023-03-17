using Dapper;

using System;

namespace SCS.HomePhotos.Data
{
    /// <summary>
    /// Represents a user filter.
    /// </summary>
    public class UserFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserFilter"/> class.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public UserFilter(int? userId = null)
        {
            UserId = userId;

            if (userId != null)
            {
                FilterType = UserPhotoScope.PersonalOnly;
            }
            else
            {
                FilterType = UserPhotoScope.Everything;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFilter"/> class.
        /// </summary>
        /// <param name="filterType">Type of the filter.</param>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="System.InvalidOperationException">A user id is required for filter type.</exception>
        public UserFilter(UserPhotoScope filterType, int? userId = null)
        {
            if (userId == null && (filterType == UserPhotoScope.PersonalOnly || filterType == UserPhotoScope.SharedAndPersonal))
            {
                throw new InvalidOperationException("A user id is required for filter type.");
            }
            FilterType = filterType;
            UserId = userId;
        }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int? UserId { get; private set; }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        public UserPhotoScope FilterType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the data range is cleared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cleared; otherwise, <c>false</c>.
        /// </value>
        public bool IsCleared
        {
            get
            {
                return UserId == null && FilterType == UserPhotoScope.Everything;
            }
        }

        /// <summary>
        /// Gets the where clause for a SQL query.
        /// </summary>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="prefixWithAnd">if set to <c>true</c> prefix with "AND".</param>
        /// <returns>The where clause SQL.</returns>
        internal (string Sql, DynamicParameters Parameters) GetWhereClause(string tableAlias = "", bool prefixWithAnd = false) 
        {
            var sql = string.Empty;
            var sep = string.IsNullOrEmpty(tableAlias) ? string.Empty : ".";
            var parameters = new DynamicParameters();

            switch (FilterType)
            {
                case UserPhotoScope.PersonalOnly:
                    sql = $"{tableAlias}{sep}UserId = @UserId ";
                    parameters.Add("UserId", UserId);
                    break;
                case UserPhotoScope.SharedAndPersonal:
                    sql = $"({tableAlias}{sep}UserId = @UserId OR {tableAlias}{sep}UserId IS NULL) ";
                    parameters.Add("UserId", UserId);
                    break;
            }

            sql = (sql.Length > 0 && prefixWithAnd ? "AND " : string.Empty) + sql;

            return (sql, parameters);
        }
    }
}
