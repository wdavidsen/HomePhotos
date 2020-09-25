using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public class AdminLogService : IAdminLogService
    {
        private readonly ILogData _logData;
        private readonly IStaticConfig _staticConfig;

        public AdminLogService(ILogData logData, IStaticConfig staticConfig)
        {
            _logData = logData;
            _staticConfig = staticConfig;
        }

        public async Task<DataList<LogEntry>> GetLatestLogEntries(PageInfo pageInfo, LogCategory? category, LogSeverity? serverity)
        {
            return await _logData.GetLogEntries(pageInfo, category, serverity, DateTime.MinValue, DateTime.Now);
        }

        public async Task AddLogEntry(LogEntry logEntry, TimeSpan doNotRepeatTimespan)
        {
            var lastEntry = await _logData.GetExistingEntry(logEntry.Message, doNotRepeatTimespan);

            if (lastEntry == null)
            {
                _logData.Insert(logEntry);
            }
            else
            {
                await _logData.UpdateAsync(logEntry);
            }            
        }

        public string AddLogEntry(LogEntry logEntry)
        {
            _logData.Insert(logEntry);
            return logEntry.Message;
        }

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

        public async Task TruncateLog()
        {
            var cutoffDate = DateTime.Now - TimeSpan.FromDays(_staticConfig.LogRetentionDays);

            await _logData.DeleteLogEntries(cutoffDate);
        }
    }
}
