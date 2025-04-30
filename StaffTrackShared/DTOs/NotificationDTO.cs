namespace StaffTrackShared.DTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FromUserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

        public string TimestampFormatted => Timestamp.ToString("yyyy-MM-dd HH:mm");
    }
}