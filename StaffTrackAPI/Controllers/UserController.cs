using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffTrackShared.DTOs;
using StaffTrackAPI.Data;
using StaffTrackAPI.Models;

namespace StaffTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly StaffTrackDbContext _context;
        private readonly IMapper _mapper; 

        public UserController(StaffTrackDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;  
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);  
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDTO>(user); 
            return Ok(userDto);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetByDepartment(int departmentId)
        {
            var usersInDepartment = await _context.Users
                                                   .Where(u => u.DepartmentId == departmentId)  
                                                   .ToListAsync();

            if (usersInDepartment == null || !usersInDepartment.Any())
            {
                return NotFound(new { Message = "No users found in this department." });
            }

            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(usersInDepartment);

            return Ok(userDtos); 
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, UserDTO userDto)
        {
            if (id != userDto.Id)
                return BadRequest();

            var user = _mapper.Map<User>(userDto);  
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
