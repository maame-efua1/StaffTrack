using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffTrackAPI.Models;
using StaffTrackShared.DTOs;
using StaffTrackAPI.Data;

namespace StaffTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly StaffTrackDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentController(StaffTrackDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetAll()
        {
            var departments = await _context.Departments.ToListAsync();
            if (departments == null || !departments.Any())
                return NotFound(new { Message = "No departments found." });

            var departmentDtos = _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
            return Ok(new { Message = "Departments retrieved successfully.", Data = departmentDtos });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDTO>> GetById(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
                return NotFound(new { Message = "Department not found." });

            var departmentDto = _mapper.Map<DepartmentDTO>(department);
            return Ok(new { Message = "Department retrieved successfully.", Data = departmentDto });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, DepartmentDTO departmentDto)
        {
            if (id != departmentDto.Id)
                return BadRequest(new { Message = "Department ID mismatch." });

            var department = _mapper.Map<Department>(departmentDto);
            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                    return NotFound(new { Message = "Department not found." });
                else
                    throw;
            }

            return Ok(new { Message = "Department updated successfully." });  // Success message here
        }

        [HttpPost("add")]
        public async Task<ActionResult<DepartmentDTO>> Add(DepartmentDTO departmentDto)
        {
            var department = _mapper.Map<Department>(departmentDto);
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var createdDepartmentDto = _mapper.Map<DepartmentDTO>(department);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, new { Message = "Department created successfully.", Data = createdDepartmentDto });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound(new { Message = "Department not found." });

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Department deleted successfully." });  // Success message here
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
