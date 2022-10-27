using System;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// The log entry DTO.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class using the domain model class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public LogEntry(Model.LogEntry entry)
        {
            Message = entry.Message;
            Severity = entry.Severity.ToString();
            Category = entry.Category.ToString();
            Timestamp = entry.Timestamp;
        }

        /// <summary>
        /// Gets or sets the log entry message.
        /// </summary>
        /// <value>
        /// The log entry message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the log entry severity.
        /// </summary>
        /// <value>
        /// The log entry severity.
        /// </value>
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the log entry ategory.
        /// </summary>
        /// <value>
        /// The log entry category.
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the log entry timestamp.
        /// </summary>
        /// <value>
        /// The log entry timestamp.
        /// </value>
        public DateTime Timestamp { get; set; }
    }
}
