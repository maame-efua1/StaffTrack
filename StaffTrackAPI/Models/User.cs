using Microsoft.AspNetCore.Identity;
using StaffTrackAPI.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffTrackAPI.Models
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
