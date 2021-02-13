using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{
    public interface IAdminLogService
    {
        string AddLogEntry(LogEntry logEntry);
        Task AddLogEntry(LogEntry logEntry, TimeSpan doNotRepeatTimespan);
        Task<DataList<LogEntry>> GetLatestLogEntries(PageInfo pageInfo, LogCategory? category, LogSeverity? serverity);
        string LogHigh(string message, LogCategory category = LogCategory.General);
        string LogNeutral(string message, LogCategory category = LogCategory.General);
        string LogElevated(string message, LogCategory category = LogCategory.General);
        string LogUrgent(string message, LogCategory category = LogCategory.General);
        Task TruncateLog();
    }
}