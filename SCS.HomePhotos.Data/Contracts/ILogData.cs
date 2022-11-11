using SCS.HomePhotos.Model;

using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The log data repository
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IDataBase" />
    public interface ILogData : IDataBase
    {
        /// <summary>
        /// Gets the log entries.
        /// </summary>
        /// <param name="pageInfo">The page information.</param>
        /// <param name="category">The category.</param>
        /// <param name="serverity">The serverity.</param>
        /// <param name="timestampStart">The timestamp start.</param>
        /// <param name="timestampEnd">The timestamp end.</param>
        /// <returns>A paged list of log entries.</returns>
        Task<DataList<LogEntry>> GetLogEntries(PageInfo pageInfo,
            LogCategory? category,
            LogSeverity? serverity,
            DateTime timestampStart,
            DateTime timestampEnd);

        /// <summary>
        /// Deletes log entries older than a specified date.
        /// </summary>
        /// <param name="olderThanDate">The cutoff date.</param>
        Task DeleteLogEntries(DateTime olderThanDate);

        /// <summary>
        /// Gets an existing log entry by message withing a specified time frame.
        /// </summary>
        /// <param name="message">The message to search for.</param>
        /// <param name="age">The max age of message.</param>
        /// <returns>The log entry.</returns>
        Task<LogEntry> GetExistingEntry(string message, TimeSpan age);
    }
}