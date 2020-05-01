using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public interface IAdminLogService
    {
        string AddLogEntry(LogEntry logEntry);
        Task<DataList<LogEntry>> GetLatestLogEntries(PageInfo pageInfo, LogCategory? category, LogSeverity? serverity);
        string LogError(string message, LogCategory category = LogCategory.General);
        string LogInformation(string message, LogCategory category = LogCategory.General);
        string LogWarning(string message, LogCategory category = LogCategory.General);
        Task TruncateLog();
    }
}