using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
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
            return await _logData.GetLogEntries(pageInfo, category, serverity, DateTime.MinValue, DateTime.UtcNow);
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
                Timestamp = DateTime.UtcNow
            });
        }

        public string LogNeutral(string message, LogCategory category = LogCategory.General)
        {
            return AddLogEntry(new LogEntry
            {
                Message = message,
                Category = category,
                Severity = LogSeverity.Neutral,
                Timestamp = DateTime.UtcNow
            });
        }

        public string LogElevated(string message, LogCategory category = LogCategory.General)
        {
            return AddLogEntry(new LogEntry
            {
                Message = message,
                Category = category,
                Severity = LogSeverity.Elevated,
                Timestamp = DateTime.UtcNow
            });
        }

        public string LogUrgent(string message, LogCategory category = LogCategory.General)
        {
            return AddLogEntry(new LogEntry
            {
                Message = message,
                Category = category,
                Severity = LogSeverity.Urgent,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task TruncateLog()
        {
            var cutoffDate = DateTime.UtcNow - TimeSpan.FromDays(_staticConfig.LogRetentionDays);

            await _logData.DeleteLogEntries(cutoffDate);
        }
    }
}
