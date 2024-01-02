using System.Globalization;

namespace API.Helpers;

public static class DateHelper
{
    public static string GetDate(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    public static bool IsValidShortDateFormat(string dateString)
    {
        return DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}