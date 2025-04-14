using Microsoft.AspNetCore.Identity;
using StaffTrack.Models.Enums;

namespace StaffTrack.DTOs
{
    public class RegisterDTO
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int DepartmentId { get; set; }

        public string StaffStatus { get; set; }

    }
}
