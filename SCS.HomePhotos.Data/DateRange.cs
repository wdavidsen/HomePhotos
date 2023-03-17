using Dapper;

using System;

namespace SCS.HomePhotos.Data
{
    /// <summary>
    /// Represents a date range.
    /// </summary>
    public struct DateRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> struct.
        /// </summary>
        public DateRange()
        {
            FromDate = DateTime.MinValue;
            ToDate = DateTime.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> struct.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        public DateRange(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> struct.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        public DateRange(DateTime? fromDate, DateTime? toDate)
        {
            FromDate = fromDate.HasValue ? fromDate.Value : DateTime.MinValue;
            ToDate = toDate.HasValue ? toDate.Value : DateTime.MaxValue;
        }

        /// <summary>
        /// Gets or sets from date.
        /// </summary>
        /// <value>
        /// From date.
        /// </value>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Converts to date.
        /// </summary>
        /// <value>
        /// To date.
        /// </value>
        public DateTime ToDate { get; set; }

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
                return FromDate != DateTime.MinValue && ToDate != DateTime.MinValue;
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
            var sep = string.IsNullOrEmpty(tableAlias) ? string.Empty : ".";
            var parameters = new DynamicParameters();

            var sql = $"{tableAlias}{sep}DateTaken BETWEEN @FromDate AND @ToDate ";

            var fromDate = FromDate;
            var toDate = ToDate;   

            if (FromDate > toDate)
            {
                (fromDate, toDate) = (toDate, fromDate);
            }

            parameters.Add("FromDate", fromDate.ToStartOfDay().ToString(Constants.DatabaseDateTimeFormat));
            parameters.Add("ToDate", toDate.ToEndOfDay().ToString(Constants.DatabaseDateTimeFormat));

            if (prefixWithAnd)
            {
                sql = "AND " + sql;
            }
            return (sql, parameters);
        }
    }
}
