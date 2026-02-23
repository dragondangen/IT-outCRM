using System.Security.Claims;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public NotificationsController(IUnitOfWork unitOfWork, ILogger<NotificationsController> logger) : base(logger)
        {
            _uow = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int limit = 50)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            limit = Math.Clamp(limit, 1, 100);
            var notifications = await _uow.Notifications.GetByUserIdAsync(userId.Value, limit);
            return Ok(notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                Link = n.Link,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }));
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var count = await _uow.Notifications.GetUnreadCountAsync(userId.Value);
            return Ok(new { count });
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            await _uow.Notifications.MarkAsReadAsync(id);
            await _uow.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            await _uow.Notifications.MarkAllAsReadAsync(userId.Value);
            return Ok();
        }

        /// <summary>
        /// Создать уведомление (системный вызов, Admin/Manager)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type ?? "info",
                Link = dto.Link,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Notifications.AddAsync(notification);
            await _uow.SaveChangesAsync();
            return Ok(new { notification.Id });
        }

        private Guid? GetUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idStr, out var id) ? id : null;
        }
    }

    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "info";
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Type { get; set; }
        public string? Link { get; set; }
    }
}
