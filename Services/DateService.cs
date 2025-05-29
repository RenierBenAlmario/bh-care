using System;
using System.Globalization;
using Barangay.Helpers;

namespace Barangay.Services
{
    public interface IDateService
    {
        string FormatDate(DateTime date, string format = "yyyy-MM-dd");
        DateTime ParseDate(string dateString);
        DateTime? ParseNullableDate(string dateString);
        bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate);
        bool IsDateEqual(DateTime date1, DateTime date2);
        bool IsDateBefore(DateTime date1, DateTime date2);
        bool IsDateAfter(DateTime date1, DateTime date2);
        string ToDateString(DateTime date);
        string FormatTime(TimeSpan time);
    }

    public class DateService : IDateService
    {
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        public string FormatDate(DateTime date, string format = "yyyy-MM-dd")
        {
            return DateTimeHelper.FormatDate(date, format);
        }

        public DateTime ParseDate(string dateString)
        {
            return DateTimeHelper.ParseDate(dateString);
        }

        public DateTime? ParseNullableDate(string dateString)
        {
            return DateTimeHelper.ParseNullableDate(dateString);
        }

        public bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            return !DateTimeHelper.IsDateBefore(date, startDate) && !DateTimeHelper.IsDateAfter(date, endDate);
        }

        public bool IsDateEqual(DateTime date1, DateTime date2)
        {
            return DateTimeHelper.AreDatesEqual(date1, date2);
        }

        public bool IsDateBefore(DateTime date1, DateTime date2)
        {
            return DateTimeHelper.IsDateBefore(date1, date2);
        }

        public bool IsDateAfter(DateTime date1, DateTime date2)
        {
            return DateTimeHelper.IsDateAfter(date1, date2);
        }

        public string ToDateString(DateTime date)
        {
            return DateTimeHelper.ToDateString(date);
        }

        public string FormatTime(TimeSpan time)
        {
            return DateTimeHelper.FormatTime(time);
        }
    }
} 