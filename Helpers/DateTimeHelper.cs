using System;
using System.Globalization;

namespace Barangay.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly string[] DateFormats = { "yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "dd-MM-yyyy" };
        private static readonly string[] TimeFormats = { @"hh\:mm", "HH:mm", "h:mm tt", "hh:mm tt" };

        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return DateTime.MinValue;

            if (DateTime.TryParseExact(dateString, DateFormats, Culture, DateTimeStyles.None, out DateTime result))
                return result;

            if (DateTime.TryParse(dateString, Culture, DateTimeStyles.None, out result))
                return result;

            return DateTime.MinValue;
        }

        public static bool AreDatesEqual(DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date;
        }

        public static bool IsDateBefore(DateTime date1, DateTime date2)
        {
            return date1.Date < date2.Date;
        }

        public static bool IsDateAfter(DateTime date1, DateTime date2)
        {
            return date1.Date > date2.Date;
        }

        public static string FormatDate(DateTime date, string format = "yyyy-MM-dd")
        {
            return date.ToString(format, Culture);
        }

        public static TimeSpan ParseTime(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return TimeSpan.Zero;

            try
            {
                // First, try direct TimeSpan parsing
                if (TimeSpan.TryParse(timeString, out TimeSpan timeResult))
                    return timeResult;

                // If direct parsing fails, clean up the string to handle common issues
                timeString = timeString.Trim();
                
                // Handle comma-separated values by taking the first part
                if (timeString.Contains(","))
                    timeString = timeString.Split(',')[0].Trim();
                    
                // Try parsing with TimeSpan formats
                foreach (var format in TimeFormats)
                {
                    if (TimeSpan.TryParseExact(timeString, format, Culture, TimeSpanStyles.None, out timeResult))
                        return timeResult;
                }
                    
                // Try parsing as DateTime and extract TimeOfDay
                string[] dateTimeFormats = { "h:mm tt", "hh:mm tt", "H:mm", "HH:mm" };
                if (DateTime.TryParseExact(timeString, dateTimeFormats, Culture, DateTimeStyles.None, out DateTime dateTime))
                    return dateTime.TimeOfDay;
                    
                // As a last resort, try generic parsing
                if (DateTime.TryParse(timeString, Culture, DateTimeStyles.None, out dateTime))
                    return dateTime.TimeOfDay;
            }
            catch (FormatException)
            {
                // Log or handle format exception
                return TimeSpan.Zero;
            }
            catch (Exception)
            {
                // Catch any other exceptions that might occur during parsing
                return TimeSpan.Zero;
            }

            // If all parsing attempts fail
            return TimeSpan.Zero;
        }

        public static string FormatTime(TimeSpan time)
        {
            try 
            {

                // Format without AM/PM first as a fallback
                return time.Hours.ToString("00") + ":" + time.Minutes.ToString("00");
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error formatting time: {ex.Message}");

                return "00:00";
            }
        }

        public static string GetFormattedDateTime(DateTime date, TimeSpan time)
        {
            return date.Add(time).ToString("MMM dd, yyyy h:mm tt", Culture);
        }

        public static DateTime? ParseNullableDate(string dateString)
        {
            var date = ParseDate(dateString);
            return date == DateTime.MinValue ? null : date;
        }

        public static string ToDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd", Culture);
        }

        public static string ToTimeString(this TimeSpan time)
        {
            return time.ToString(@"hh\:mm", Culture);
        }

        public static DateTime StartOfDay(DateTime date)
        {
            return date.Date;
        }

        public static DateTime EndOfDay(DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }

        // Helper for String to DateTime property conversion
        public static DateTime ToDateTimeOrDefault(string value, DateTime defaultValue = default)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;
                
            return ParseDate(value);
        }
        
        // Helper for DateTime to string property conversion
        public static string ToStringOrEmpty(this DateTime? value, string format = "yyyy-MM-dd")
        {
            if (!value.HasValue)
                return string.Empty;
                
            return value.Value.ToString(format, Culture);
        }

        public static bool IsDateGreaterThanOrEqual(DateTime date1, DateTime date2)
        {
            return date1.Date >= date2.Date;
        }

        public static bool IsDateLessThanOrEqual(DateTime date1, DateTime date2)
        {
            return date1.Date <= date2.Date;
        }

        public static bool IsDateEqual(DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date;
        }
    }
} 


