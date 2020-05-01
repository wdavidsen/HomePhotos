using Dapper;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public class LogData : DataBase, ILogData
    {
        public LogData(IStaticConfig staticConfig) : base(staticConfig) { }

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

        public async Task DeleteLogEntries(DateTime olderThanDate)
        {
            var oldEntries = await GetListAsync<LogEntry>("WHERE Timestamp > @Timestamp", new { Timestamp = olderThanDate });

            foreach (var entry in oldEntries)
            {
                await DeleteAsync(entry);
            }
        }
    }
}
