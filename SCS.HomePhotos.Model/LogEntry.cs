using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The log severity.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// The neutral severity.
        /// </summary>
        Neutral = 0,
        /// <summary>
        /// The elevated severity.
        /// </summary>
        Elevated = 1,
        /// <summary>
        /// The high severity.
        /// </summary>
        High = 2,
        /// <summary>
        /// The urgent severity.
        /// </summary>
        Urgent = 3
    }

    /// <summary>
    /// The log category.
    /// </summary>
    public enum LogCategory
    {
        /// <summary>
        /// The general category.
        /// </summary>
        General = 0,
        /// <summary>
        /// The security category.
        /// </summary>
        Security = 1,
        /// <summary>
        /// The index category.
        /// </summary>
        Index = 2,
        /// <summary>
        /// The upload category.
        /// </summary>
        Upload = 3
    }

    /// <summary>
    /// The log entity.
    /// </summary>
    [Table("Log")]
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the log identifier.
        /// </summary>
        /// <value>
        /// The log identifier.
        /// </value>
        [Key]
        public int? LogId { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        /// <value>
        /// The log message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the log severity.
        /// </summary>
        /// <value>
        /// The log severity.
        /// </value>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the log category.
        /// </summary>
        /// <value>
        /// The log category.
        /// </value>
        public LogCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the log timestamp.
        /// </summary>
        /// <value>
        /// The log timestamp.
        /// </value>
        public DateTime Timestamp { get; set; }
    }
}
