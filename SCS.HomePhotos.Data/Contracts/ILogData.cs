using SCS.HomePhotos.Model;
using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    public interface ILogData : IDataBase
    {
        Task<DataList<LogEntry>> GetLogEntries(PageInfo pageInfo,
            LogCategory? category,
            LogSeverity? serverity,
            DateTime timestampStart,
            DateTime timestampEnd);

        Task DeleteLogEntries(DateTime olderThanDate);

        Task<LogEntry> GetExistingEntry(string message, TimeSpan age);
    }
}