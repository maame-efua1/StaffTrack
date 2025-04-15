using Microsoft.AspNetCore.Identity;
using StaffTrack.API.Models.Enums;

namespace StaffTrack.API.DTOs
{
    public class UserDTO : IdentityUser
    {
        public string FullName { get; set; }

        public int DepartmentId { get; set; }

        public DepartmentDTO Department { get; set; }

        public StaffStatus StaffStatus { get; set; }

    }
}
