using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Mapping;
using FinalProject3.Models;
using FinalProject32.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Security.Claims;

namespace FinalProject3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(FP3Context context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly FP3Context _context = context;


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> CreatNewChat ([FromBody] IdInput idInput)
        {
            var user2id = idInput.id;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            ChatNew chat = new ChatNew()
            {
                User1Id = userId,
                User2Id = user2id,
            };
            var upChat = await chat.MakeNewChat(userManager);
            if (upChat is null)
            {
                return BadRequest();
            }
            var exists = await _context.Chat.FindAsync(upChat.Id);
            if (exists is not null)
            {
                return BadRequest("Chat exists");
            }

            await _context.Chat.AddAsync(upChat);
            var user1 = await _context.Users.FindAsync(userId);
            var user2 = await _context.Users.FindAsync (user2id);
            if (user1 is null || user2 is null)
            {
                return BadRequest();
            }
            user1.Chats.Add(upChat);
            user2.Chats.Add(upChat);
            await _context.SaveChangesAsync();
            return Ok(upChat.Id);

        }

        [HttpGet("ByChatId/{ChatID}")]
        [Authorize]
        public async Task<ActionResult<Chat>> GetChatID(string ChatId)
        {
            var chat = await _context.Chat.Include(c => c.messages).FirstOrDefaultAsync(c => c.Id == ChatId);
            if (chat == null)
            {
                return NotFound();
            }
            return Ok(chat.ToDisplay());
        }

        [HttpGet("notFollowingChats")]
        [Authorize]
        public async Task<ActionResult<AppUserDisplay>> GetNotFollowedChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }

            var user = await _context.Users.Include(u => u.Chats).FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return Unauthorized();
            }

            var chattingWith = user.Chats.Select(c => c.getOtherUser(userId)).ToList();
            var Following = user.FollowingId;
            var chatNotFollowedId = new List<string>();
            foreach (var chat in chattingWith)
            {
                if (!Following.Contains(chat))
                {
                    chatNotFollowedId.Add(chat);
                }
            }
            var chatNotFollowed = new List<AppUserDisplay>();
            foreach (var id in chatNotFollowedId)
            {
                var chatter = await _context.Users.Include(u => u.Chats).FirstOrDefaultAsync(u => u.Id == id);
                if (chatter is not null)
                {
                    var userDisplay = await chatter.UsertoDisplay(_context, user);
                    if (userDisplay is not null)
                    {
                        chatNotFollowed.Add(userDisplay);
                    }
                }
            }
            return Ok(chatNotFollowed);
        }




        [HttpPost("Message")]
        [Authorize]
        public async Task<ActionResult> SendMessage([FromBody] MessageNew newmesage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (userName is null)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.Include(u => u.Chats).FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return NotFound("User not found");
            Message newMessage = newmesage.MakeMessage(userId, userName);
            var chat = await _context.Chat.FindAsync(newmesage.ChatId);
            if (chat is null) return BadRequest("Chat not found");
            chat.messages.Add(newMessage);
            await _context.Message.AddAsync(newMessage);

            var secondUser = chat.getOtherUser(userId);
            var user2 = await _context.Users.FirstOrDefaultAsync(u => u.Id == secondUser);
            if (user2?.LastActive != null)
            {
                DateTime lastActiveDateTime;
                bool parsed = DateTime.TryParseExact(user2.LastActive, "yyyy-MM-dd-HH-mm",
                                                     null, DateTimeStyles.None, out lastActiveDateTime);
                if (parsed)
                {
                    TimeSpan timeDifference = DateTime.UtcNow - lastActiveDateTime;
                    if (timeDifference.TotalMinutes >= 10)
                    {
                        var notification = await newMessage.CreatMessageNotification(chat, userId, _context);
                        await _context.Notification.AddAsync(notification);
                        user2.Notifications.Add(notification);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Ok(newMessage);
        }
    }
}
