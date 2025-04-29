namespace StaffTrackAPI.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetIso8601WeekOfYear(this DateTime date)
        {
            var day = (int)date.DayOfWeek;
            if (day == 0) day = 7; // Sunday is 0, make it 7
            date = date.AddDays(3 - (day % 7)); // Adjust to Thursday of the week
            return (date.DayOfYear - 1) / 7 + 1;
        }
    }
}