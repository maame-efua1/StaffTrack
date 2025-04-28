using System.ComponentModel.DataAnnotations;

namespace StaffTrackAPI.Models
{
    public class Department
    {
        [Key]

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
