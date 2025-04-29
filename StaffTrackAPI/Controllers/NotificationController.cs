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
    public class NotificationController : ControllerBase
    {
        private readonly StaffTrackDbContext _context;
        private readonly IMapper _mapper;

        public NotificationController(StaffTrackDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotifications()
        {
            var notifications = await _context.Notifications
                .OrderByDescending(n => n.Timestamp)
                .Take(10) // Limit to 10 recent notifications
                .ToListAsync();

            if (notifications == null || !notifications.Any())
                return NotFound(new { Message = "No notifications found." });

            var notificationDtos = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
            return Ok(new { Message = "Notifications retrieved successfully.", Data = notificationDtos });
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var notifications = await _context.Notifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            if (notifications == null || !notifications.Any())
                return Ok(new { Message = "No unread notifications to mark as read." });

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "All notifications marked as read." });
        }
    }
}