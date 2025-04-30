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
    public class ReportController : ControllerBase
    {
        private readonly StaffTrackDbContext _context;
        private readonly IMapper _mapper;

        public ReportController(StaffTrackDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportDTO>>> GetAll()
        {
            var reports = await _context.Reports.ToListAsync();
            if (reports == null || !reports.Any())
                return NotFound(new { Message = "No reports found." });

            var reportDtos = _mapper.Map<IEnumerable<ReportDTO>>(reports);
            return Ok(new { Message = "Reports retrieved successfully.", Data = reportDtos });
        }

        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<ReportDTO>>> GetRecent()
        {
            var reports = await _context.Reports
                .OrderByDescending(r => r.SubmittedAt)
                .Take(5) // Limit to 5 recent reports
                .ToListAsync();

            if (reports == null || !reports.Any())
                return NotFound(new { Message = "No recent reports found." });

            var reportDtos = _mapper.Map<IEnumerable<ReportDTO>>(reports);
            return Ok(reportDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReportDTO>> GetById(int id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
                return NotFound(new { Message = "Report not found." });

            var reportDto = _mapper.Map<ReportDTO>(report);
            return Ok(new { Message = "Report retrieved successfully.", Data = reportDto });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReportDTO>>> GetByUser(string userId)
        {
            var reports = await _context.Reports
                .Where(r => r.UserId == userId)
                .ToListAsync();

            if (reports == null || !reports.Any())
                return NotFound(new { Message = "No reports found for this user." });

            var reportDtos = _mapper.Map<IEnumerable<ReportDTO>>(reports);
            return Ok(new { Message = "User reports retrieved successfully.", Data = reportDtos });
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound(new { Message = "Report not found." });

            report.Status = "Approved";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Report approved successfully." });
        }

        [HttpPost("add")]
        public async Task<ActionResult<ReportDTO>> Add(ReportDTO reportDto)
        {
            var report = _mapper.Map<Report>(reportDto);
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            var createdReportDto = _mapper.Map<ReportDTO>(report);
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, new { Message = "Report created successfully.", Data = createdReportDto });
        }

        [HttpPost("addbulk")]
        public async Task<IActionResult> CreateReports([FromBody] List<ReportDTO> reports)
        {
            if (reports == null || !reports.Any())
            {
                return BadRequest("No reports provided.");
            }

            try
            {
                // Map DTOs to entities
                var reportEntities = reports.Select(dto => new Report
                {
                    Id = dto.Id, // Will be ignored if Id is 0 (auto-incremented by database)
                    UserId = dto.UserId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    WeekNumber = dto.WeekNumber,
                    FilePath = dto.FilePath,
                    SubmittedAt = dto.SubmittedAt,
                    Status = dto.Status
                }).ToList();

                // Add reports to the database
                _context.Reports.AddRange(reportEntities);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Reports added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving reports: {ex.Message}");
            }
        }

            [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, ReportDTO reportDto)
        {
            if (id != reportDto.Id)
                return BadRequest(new { Message = "Report ID mismatch." });

            var report = _mapper.Map<Report>(reportDto);
            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
                    return NotFound(new { Message = "Report not found." });
                else
                    throw;
            }

            return Ok(new { Message = "Report updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound(new { Message = "Report not found." });

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Report deleted successfully." });
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}