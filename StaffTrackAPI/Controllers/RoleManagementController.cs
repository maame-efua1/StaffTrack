using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using StaffTrackAPI.Models;
using System.Threading.Tasks;

namespace StaffTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleManagementController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleManagementController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name is required.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return BadRequest("Role already exists.");
            }

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Role created successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("UpdateRole/{roleName}")]
        public async Task<IActionResult> UpdateRole(string roleName, [FromBody] string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Role updated successfully." });
            }

            return BadRequest(result.Errors);
        }


        [HttpDelete("DeleteRole/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role deleted successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("assign/{userId}/{roleName}")]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return NotFound("Role not found.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return BadRequest(removeResult.Errors);
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, roleName);
            if (addResult.Succeeded)
            {
                return Ok(new { Message = $"Role {roleName} assigned to user {user.UserName}." });
            }

            return BadRequest(addResult.Errors);
        }


        [HttpPost("remove/{userId}/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = $"Role {roleName} removed from user {user.UserName}." });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("user/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        // Check if a user has a specific role
        [HttpGet("user/{userId}/hasrole/{roleName}")]
        public async Task<IActionResult> CheckUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var hasRole = await _userManager.IsInRoleAsync(user, roleName);
            return Ok(new { HasRole = hasRole });
        }
    }
}
