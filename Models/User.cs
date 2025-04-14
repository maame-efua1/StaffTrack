using Microsoft.AspNetCore.Identity;
using StaffTrack.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffTrack.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }

        public StaffStatus StaffStatus { get; set; }

        public ICollection<Report>? Reports { get; set; }
    }
}
