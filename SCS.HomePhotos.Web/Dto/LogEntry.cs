using System;

namespace SCS.HomePhotos.Web.Dto
{
    public class LogEntry
    {
        public LogEntry(Model.LogEntry entry)
        {
            Message = entry.Message;
            Severity = entry.Severity.ToString();
            Category = entry.Category.ToString();
            Timestamp = entry.Timestamp;
        }

        public string Message { get; set; }

        public string Severity { get; set; }

        public string Category { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
