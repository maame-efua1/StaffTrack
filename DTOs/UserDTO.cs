using Microsoft.AspNetCore.Identity;
using StaffTrack.Models.Enums;

namespace StaffTrack.DTOs
{
    public class UserDTO : IdentityUser
    {
        public string FullName { get; set; }

        public int DepartmentId { get; set; }

        public DepartmentDTO Department { get; set; }

        public StaffStatus StaffStatus { get; set; }

    }
}
