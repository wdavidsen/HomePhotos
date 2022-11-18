using SCS.HomePhotos.Data;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;

using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// The admin log service.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Service.Contracts.IAdminLogService" />
    public class AdminLogService : HomePhotosService, IAdminLogService
    {
        private readonly ILogData _logData;
        private readonly IStaticConfig _staticConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminLogService"/> class.
        /// </summary>
        /// <param name="logData">The log data.</param>
        /// <param name="staticConfig">The static configuration.</param>
        public AdminLogService(ILogData logData, IStaticConfig staticConfig)
        {
            _logData = logData;
            _staticConfig = staticConfig;
        }

        /// <summary>
        /// Gets the latest log entries.
        /// </summary>
        /// <param name="pageInfo">The page information.</param>
        /// <param name="category">The category.</param>
        /// <param name="serverity">The severity.</param>
        /// <returns></returns>
        public async Task<DataList<LogEntry>> GetLatestLogEntries(PageInfo pageInfo, LogCategory? category, LogSeverity? serverity)
        {
            return await _logData.GetLogEntries(pageInfo, category, serverity, DateTime.MinValue, DateTime.Now);
        }

        /// <summary>
        /// Adds the log entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="doNotRepeatTimespan">The do-not-repeat timespan. Will not log entry if the same info was already logged in within the timespan specified.</param>
        public async Task AddLogEntry(LogEntry logEntry, TimeSpan doNotRepeatTimespan)
        {
            var lastEntry = await _logData.GetExistingEntry(logEntry.Message, doNotRepeatTimespan);

            if (lastEntry == null)
            {
                await _logData.InsertAsync(logEntry);
            }
            else
            {
                await _logData.UpdateAsync(logEntry);
            }
        }

        /// <summary>
        /// Adds the log entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>
        /// The logged message.
        /// </returns>
        public string AddLogEntry(LogEntry logEntry)
        {
            _logData.Insert(logEntry);
            return logEntry.Message;
        }

        /// <summary>
        /// Logs with high importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The logged message.
        /// </returns>
        public string LogHigh(string message, LogCategory category = LogCategory.General)
        {
            return AddLogEntry(new LogEntry
            {
                Message = message,
                Category = category,
                Severity = LogSeverity.High,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Logs with neutral importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The logged message.
        /// </returns>
        public string LogNeutral(string message, LogCategory category = LogCategory.General)
        {
            return AddLogEntry(new LogEntry
            {
                Message = message,
                Category = category,
                Severity = LogSeverity.Neutral,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Logs with elevated importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The logged message.
        /// </returns>
        public string LogElevated(string message, LogCategory category = LogCategory.General)
        {
            return AddLogEntry(new LogEntry
            {
                Message = message,
                Category = category,
                Severity = LogSeverity.Elevated,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Logs with highest urgency.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The logged message.
        /// </returns>
        public string LogUrgent(string message, LogCategory category = LogCategory.General)
        {
            return AddLogEntry(new LogEntry
            {
                Message = message,
                Category = category,
                Severity = LogSeverity.Urgent,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Truncates the log.
        /// </summary>
        public async Task TruncateLog()
        {
            var cutoffDate = DateTime.Now - TimeSpan.FromDays(_staticConfig.LogRetentionDays);

            await _logData.DeleteLogEntries(cutoffDate);
        }
    }
}
