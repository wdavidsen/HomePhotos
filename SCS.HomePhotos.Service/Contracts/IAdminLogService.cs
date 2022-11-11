using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;

using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{

    /// <summary>
    /// The admin log service.
    /// </summary>
    public interface IAdminLogService
    {
        /// <summary>
        /// Adds the log entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>The logged message.</returns>
        string AddLogEntry(LogEntry logEntry);

        /// <summary>
        /// Adds the log entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="doNotRepeatTimespan">The do-not-repeat timespan. Will not log entry if the same info was already logged in within the timspan specified.</param>
        /// <returns>The logged message.</returns>
        Task AddLogEntry(LogEntry logEntry, TimeSpan doNotRepeatTimespan);

        /// <summary>
        /// Gets the latest log entries.
        /// </summary>
        /// <param name="pageInfo">The page information.</param>
        /// <param name="category">The category.</param>
        /// <param name="serverity">The serverity.</param>
        /// <returns>The logged message.</returns>
        Task<DataList<LogEntry>> GetLatestLogEntries(PageInfo pageInfo, LogCategory? category, LogSeverity? serverity);

        /// <summary>
        /// Logs with high importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>The logged message.</returns>
        string LogHigh(string message, LogCategory category = LogCategory.General);

        /// <summary>
        /// Logs with neutral importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>The logged message.</returns>
        string LogNeutral(string message, LogCategory category = LogCategory.General);

        /// <summary>
        /// Logs with elevated importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>The logged message.</returns>
        string LogElevated(string message, LogCategory category = LogCategory.General);

        /// <summary>
        /// Logs with highest urgency.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>The logged message.</returns>
        string LogUrgent(string message, LogCategory category = LogCategory.General);

        /// <summary>
        /// Truncates the log.
        /// </summary>
        /// <returns>A void task.</returns>
        Task TruncateLog();
    }
}