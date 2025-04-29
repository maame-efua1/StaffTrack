using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffTrackAPI.Data;
using StaffTrackAPI.Extensions;
using StaffTrackShared.DTOs;

namespace StaffTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly StaffTrackDbContext _context;

        public ProgressController(StaffTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ProgressDTO>> GetProgress()
        {
            // Calculate total weeks in the current year
            var currentYear = DateTime.UtcNow.Year;
            var totalWeeks = 52; // Simplified: assume 52 weeks in a year

            // Fetch reports for the current year
            var reports = await _context.Reports
                .Where(r => r.SubmittedAt.Year == currentYear)
                .ToListAsync();

            // Calculate submitted weeks
            var submittedWeeks = reports
                .Select(r => r.WeekNumber)
                .Distinct()
                .Count();

            // Calculate percentage
            var percentage = (double)submittedWeeks / totalWeeks * 100;

            // Calculate report count
            var totalReports = reports.Count;
            var reportCount = $"{submittedWeeks}/{totalWeeks} weeks";

            // Calculate streak (consecutive weeks with submissions)
            var streak = 0;
            var currentWeek = DateTime.UtcNow.GetIso8601WeekOfYear();
            var weeksWithReports = reports.Select(r => r.WeekNumber).Distinct().OrderByDescending(w => w).ToList();

            if (weeksWithReports.Any())
            {
                var week = currentWeek;
                while (weeksWithReports.Contains(week))
                {
                    streak++;
                    week--;
                }
            }

            var progressDto = new ProgressDTO
            {
                Percentage = Math.Round(percentage, 1),
                ReportCount = reportCount,
                Streak = $"{streak} weeks"
            };

            return Ok(new { Message = "Progress retrieved successfully.", Data = progressDto });
        }
    }
}