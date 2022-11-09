using Dapper;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The log data repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Core.DataBase" />
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.ILogData" />
    public class LogData : DataBase, ILogData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public LogData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets the log entries.
        /// </summary>
        /// <param name="pageInfo">The page information.</param>
        /// <param name="category">The category.</param>
        /// <param name="serverity">The severity.</param>
        /// <param name="timestampStart">The timestamp start.</param>
        /// <param name="timestampEnd">The timestamp end.</param>
        /// <returns>A paged list of log entries.</returns>
        public async Task<DataList<LogEntry>> GetLogEntries(PageInfo pageInfo,
            LogCategory? category,
            LogSeverity? serverity,
            DateTime timestampStart,
            DateTime timestampEnd)
        {
            var where = "WHERE Timestamp >= @TimetampStart AND Timestamp <= @TimetampEnd ";

            var parameters = new DynamicParameters();
            parameters.Add("@TimetampStart", timestampStart);
            parameters.Add("@TimetampEnd", timestampEnd);

            if (category != null)
            {
                where += "AND Category = @Category ";
                parameters.Add("@Category", (int)category);
            }

            if (serverity != null)
            {
                where += "AND Severity = @Severity ";
                parameters.Add("@Severity", (int)serverity);
            }

            var orderBy = "Timestamp" + (pageInfo.SortDescending ? " DESC" : "");
            var data = await GetListPagedAsync<LogEntry>(where, parameters, orderBy, pageInfo.PageNum, pageInfo.PageSize);
            pageInfo.TotalRecords = (int)await GetRecordCount<LogEntry>(where, parameters);

            return new DataList<LogEntry>(data, pageInfo);
        }

        /// <summary>
        /// Deletes log entries older than a specified date.
        /// </summary>
        /// <param name="olderThanDate">The cutoff date.</param>
        public async Task DeleteLogEntries(DateTime olderThanDate)
        {
            var oldEntries = await GetListAsync<LogEntry>("WHERE Timestamp < @Timestamp", new { Timestamp = olderThanDate });

            foreach (var entry in oldEntries)
            {
                await DeleteAsync(entry);
            }
        }

        /// <summary>
        /// Gets an existing log entry by message withing a specified time frame.
        /// </summary>
        /// <param name="message">The message to search for.</param>
        /// <param name="age">The max age of message.</param>
        /// <returns>The log entry.</returns>
        public async Task<LogEntry> GetExistingEntry(string message, TimeSpan age)
        {
            var timespanCutoff = DateTime.Now - age;
            var parameters = new { Message = message, TimespanCutoff = timespanCutoff };

            var messages = await GetListPagedAsync<LogEntry>("Message = @Message AND Timespan >= @TimespanCutoff", parameters, "Timespan DESC", 1, 1);

            return messages != null ? messages.FirstOrDefault() : null;
        }
    }
}
