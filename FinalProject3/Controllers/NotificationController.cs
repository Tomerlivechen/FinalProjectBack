using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Mapping;
using FinalProject3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FinalProject3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(FP3Context context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly FP3Context _context = context;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<NotificationDisplay>>> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _context.Notification.Where(n => n.Notified != null && n.Notified.Id == userId && !n.Hidden).Select(n => n.ToDisplay()).ToListAsync();
            return (notifications);
        }

        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> AgeOutNotifications()
        {
            var notificationsToAgeOut = await _context.Notification.ToListAsync();
            foreach (var notification in notificationsToAgeOut)
            {
                notification.AgeOut(); 
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Problem(ex.Message);
            }

            return Ok();
        }


        [HttpPost]
        public async Task<ActionResult> PostNotification(NotificationNew notificationNew)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(notificationNew.ReferenceId))
            {
                return BadRequest(ModelState);
            }
            Notification notification = await notificationNew.AddNotification(context);
            AppUser? notified = null;
            Interaction? interaction = null;
            interaction = await _context.Comment.FindAsync(notificationNew.ReferenceId);
            if (interaction is null)
            {
                interaction = await _context.Post.FindAsync(notificationNew.ReferenceId);
            }
            if (interaction is null)
            {
                return NotFound("Comment not found");
            }
                switch (notificationNew.Type)
            {
                case "Comment":
                    notified = await userManager.FindByIdAsync(interaction.Author.Id);
                    break;
                case "Message":
                    Chat? chat = await _context.Chat.FindAsync(notificationNew.ReferenceId);
                    if (chat == null) return NotFound("Chat not found");
                    notified = await userManager.FindByIdAsync(chat.NotificatioToWho());
                    break;
                default:
                    return BadRequest("Invalid notification type.");
            }
            if (notified is null)
            {
                return NotFound("User Not found");
            }
            await _context.Notification.AddAsync(notification);
            notified.Notifications.Add(notification);
            await userManager.UpdateAsync(notified);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Problem(ex.Message);
            }

            return Ok(notification.ToDisplay());
        }

        [HttpPut("Update/{notificationID}")]
        [Authorize]
        public async Task<ActionResult> EditNotification(string notificationID, [FromBody] BoolInput remove)
        {
            bool seen = true;
            bool hide = remove.Input;
            var notification = await _context.Notification.FindAsync(notificationID);
            if (notification == null)
            {
                return NotFound();
            }
            if (seen)
            {
                notification.Viewed();
            }
            if (hide)
            {
                notification.Hide();
            }

            _context.Entry(notification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(new { error ="A concurrency conflict occurred. The record was modified by another user." ,ex});
            }
            catch (DbUpdateException ex)
            {
                return Problem(ex.Message);
            }

            return Ok(notification);

        }
        private bool NotificationExists(string id)
        {
            return _context.Notification.Any(e => e.Id == id);
        }
    }
}
