using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffTrackAPI.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public int WeekNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
