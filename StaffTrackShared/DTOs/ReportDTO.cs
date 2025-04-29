namespace StaffTrackShared.DTOs
{
    public class ReportDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int WeekNumber { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; } = string.Empty;

        // Fields expected by the frontend
        public string Title => $"Week {WeekNumber} Report"; // Computed title
        public string SubmissionDate => SubmittedAt.ToString("yyyy-MM-dd"); // Formatted date
    }
}