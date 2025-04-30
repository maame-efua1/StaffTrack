using AutoMapper;
using Microsoft.AspNetCore.Authorization; // For role-based authorization
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffTrackAPI.Models;
using StaffTrackShared.DTOs;
using StaffTrackAPI.Data;
using System.Security.Claims; // For getting UserId from claims

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
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetAll()
        {
            var notifications = await _context.Notifications.ToListAsync();
            if (notifications == null || !notifications.Any())
                return NotFound(new { Message = "No notifications found." });

            var notificationDtos = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
            return Ok(notificationDtos);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotifications(string userId)
        {
            var notifications = await _context.Notifications
                .Where(r => r.UserId == userId && r.IsRead == false)
                .ToListAsync();

            if (notifications == null || !notifications.Any())
                return NotFound(new { Message = "No notifications found for this user." });

            var notificationDtos = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
            return Ok(notificationDtos);
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "UserId is required." });
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead) // Filter by UserId and unread
                .ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                return NotFound(new { Message = "No unread notifications found for this user." });
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "All notifications marked as read for this user." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationDTO notificationDto)
        {
            if (notificationDto == null || string.IsNullOrEmpty(notificationDto.UserId) || string.IsNullOrEmpty(notificationDto.Message))
            {
                return BadRequest(new { Message = "UserId and Message are required." });
            }

            if (!string.IsNullOrEmpty(notificationDto.FromUserId))
            {
                var fromUserExists = await _context.Users.AnyAsync(u => u.Id == notificationDto.FromUserId);
                if (!fromUserExists)
                {
                    return BadRequest(new { Message = "Invalid FromUserId. Sender does not exist." });
                }
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == notificationDto.UserId);
            if (!userExists)
            {
                return BadRequest(new { Message = "Invalid UserId. User does not exist." });
            }

            try
            {
                var notification = _mapper.Map<Notification>(notificationDto);
                notification.Timestamp = DateTime.UtcNow;
                notification.IsRead = false;

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                var createdNotificationDto = _mapper.Map<NotificationDTO>(notification);
                return CreatedAtAction(nameof(GetNotifications), new { userId = notification.UserId }, createdNotificationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred while creating the notification: {ex.Message}" });
            }
        }
    }
}