

namespace StaffTrackShared.DTOs
{
    public class ReportDTO
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public UserDTO User { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int WeekNumber { get; set; }

        public string FilePath { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
