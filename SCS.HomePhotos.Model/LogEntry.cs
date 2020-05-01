using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    public enum LogSeverity
    {
        Neutral = 0,
        Elevated = 1,
        High = 2,
        Urgent = 3
    }

    public enum LogCategory
    {
        General = 0,
        Security = 1,
        Index = 2,
        Upload = 3
    }

    [Table("Log")]
    public class LogEntry
    {
        [Key]
        public int? LogId { get; set; }

        public string Message { get; set; }

        public LogSeverity Severity { get; set; }

        public LogCategory Category { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
