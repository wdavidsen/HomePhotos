using System;
using System.Globalization;

namespace SCS.HomePhotos
{
    /// <summary>
    /// Extension methods to DateTime type.
    /// </summary>
    public static class DateExtensions
    {
        /// <summary>
        /// Converts date/time day to the same day's beginning date/time.
        /// </summary>
        /// <param name="value">The date/time to use.</param>
        /// <returns>The adjusted date/time.</returns>
        public static DateTime ToStartOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Converts date/time day to the same day's ending date/time.
        /// </summary>
        /// <param name="value">The date/time to use.</param>
        /// <returns>The adjusted date/time.</returns>
        public static DateTime ToEndOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 999);
        }

        /// <summary>
        /// Converts to string with provided format. Returns an empty string for a null value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="format">The format to apply.</param>
        /// <returns>
        /// A date string with format applied.
        /// </returns>
        public static string ToString(this DateTime? value, string format)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return ((DateTime)value).ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts date to a short date string. Returns an empty string for a null value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A short date string.
        /// </returns>
        public static string ToShortDateString(this DateTime? value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return ((DateTime)value).ToShortDateString();
        }
    }
}
