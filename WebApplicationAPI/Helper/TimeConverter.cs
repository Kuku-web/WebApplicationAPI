namespace WebApplicationAPI.Helper
{
    public static class TimeConverter
    {
        public static DateTime GetNextRefreshTimeCET()
        {
            // Timezone conversion: Convert current UTC time to CET
            var cetZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var cetNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cetZone);

            // Calculate the next 16:00 CET as frankfurter refreshed at 16
            var nextRefresh = new DateTime(cetNow.Year, cetNow.Month, cetNow.Day, 16, 0, 0, cetNow.Kind);

            // If it's already past 16:00 CET today, use the next day
            if (cetNow > nextRefresh)
            {
                nextRefresh = nextRefresh.AddDays(1);
            }

            // Convert back to UTC
            return TimeZoneInfo.ConvertTimeToUtc(nextRefresh, cetZone);
        }
    }
}
