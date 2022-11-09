using System;

namespace SCS.HomePhotos.Data
{
    /// <summary>
    /// Represents a date range.
    /// </summary>
    public struct DateRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> struct.
        /// </summary>
        public DateRange()
        {
            FromDate = DateTime.MinValue;
            ToDate = DateTime.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> struct.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        public DateRange(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRange"/> struct.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        public DateRange(DateTime? fromDate, DateTime? toDate)
        {
            FromDate = fromDate.HasValue ? fromDate.Value : DateTime.MinValue;
            ToDate = toDate.HasValue ? toDate.Value : DateTime.MaxValue;
        }

        /// <summary>
        /// Gets or sets from date.
        /// </summary>
        /// <value>
        /// From date.
        /// </value>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Converts to date.
        /// </summary>
        /// <value>
        /// To date.
        /// </value>
        public DateTime ToDate { get; set; }
    }
}
