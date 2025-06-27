using System.Globalization;

namespace ExpenseTracker.API.Helpers
{
    public static class TimeZoneHelper
    {
        private const string IndiaStandardTimeWindows = "India Standard Time";
        private const string IndiaStandardTimeIana = "Asia/Kolkata";
        public const string DateTimeFormat = "dd-MM-yyyy HH:mm";
        private static readonly TimeZoneInfo IstTimeZone;

        static TimeZoneHelper()
        {
            try { IstTimeZone = TimeZoneInfo.FindSystemTimeZoneById(IndiaStandardTimeWindows); }
            catch (TimeZoneNotFoundException) { IstTimeZone = TimeZoneInfo.FindSystemTimeZoneById(IndiaStandardTimeIana); }
        }

        public static DateTimeOffset ConvertUtcToIst(DateTimeOffset utcDateTime)
        {
            return TimeZoneInfo.ConvertTime(utcDateTime, IstTimeZone);
        }

        public static DateTimeOffset ConvertIstStringToUtc(string istDateTimeString)
        {
            if (!DateTime.TryParseExact(istDateTimeString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedIstDateTime))
            {
                // This will be caught by the service and returned as a 400 Bad Request
                throw new FormatException($"Invalid date/time format: '{istDateTimeString}'. Expected '{DateTimeFormat}'.");
            }
            
            // Assumes the parsed time is in IST and convert it to its UTC equivalent
            return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(parsedIstDateTime, IstTimeZone));
        }
    }
}