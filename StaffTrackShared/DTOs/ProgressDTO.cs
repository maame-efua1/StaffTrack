namespace StaffTrackShared.DTOs
{
    public class ProgressDTO
    {
        public double Percentage { get; set; }
        public string ReportCount { get; set; } = string.Empty;
        public string Streak { get; set; } = string.Empty;
    }
}