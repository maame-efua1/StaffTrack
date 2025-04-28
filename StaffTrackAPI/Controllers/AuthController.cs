using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StaffTrackShared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using StaffTrackAPI.Models.Enums;
using StaffTrackAPI.Models;

namespace StaffTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            var staffStatus = Enum.TryParse<StaffStatus>(register.StaffStatus, out var parsedStatus)
                      ? parsedStatus
                      : StaffStatus.Staff;

            var user = new User
            {
                FullName = register.FullName,
                Email = register.Email,
                DepartmentId = register.DepartmentId,
                StaffStatus = staffStatus,
                UserName = register.Email
            };

            // Generate a random password
            var generatedPassword = GenerateRandomPassword();

            var result = await _userManager.CreateAsync(user, generatedPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            return Ok(new
            {
                Message = "Registration successful.",
                Password = generatedPassword
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = await GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var fullName = jwtToken.Claims.FirstOrDefault(x => x.Type == "FullName")?.Value;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var role = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

                return Ok(new
                {
                    IsValid = true,
                    FullName = fullName,
                    Email = email,
                    Role = role
                });
            }
            catch (Exception)
            {
                return Unauthorized(new { IsValid = false, Message = "Invalid or expired token." });
            }
        }


        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, role),
        new Claim("FullName", user.FullName ?? "")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireHours = int.Parse(_configuration["Jwt:ExpireHours"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(expireHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GenerateRandomPassword(int length = 10)
        {
            var random = new Random();
            var passwordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
            var password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = passwordChars[random.Next(passwordChars.Length)];
            }

            return new string(password);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var newPassword = GenerateRandomPassword(); 

            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!resetResult.Succeeded)
            {
                return BadRequest(resetResult.Errors);
            }

            return Ok(new
            {
                Message = "Password reset successful. Use the new password to log in.",
                NewPassword = newPassword
            });
        }


    }
}
