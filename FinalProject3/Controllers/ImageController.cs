using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Mapping;
using FinalProject3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace FinalProject3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController (FP3Context context) : ControllerBase
    {

        private readonly FP3Context _context = context;

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AppImageDisplay>> sendImage(StringInput imgURL)
        {
            var imageURL = imgURL.Input;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            AppImage image = new AppImage();
            image.Url = imageURL;
            image.Public = false;
            image.user = currentUser;
            image.Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
            await _context.AppImages.AddAsync(image);
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




        [HttpGet("byId/{UserId}")]
        [Authorize]
        public async Task<ActionResult<List<AppImageDisplay>>> GetUserImages(string UserId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            var images = await _context.AppImages.Include(i => i.user).Where(i => i.user != null && i.user.Id == UserId).Select(i=> i.ToDisplay(_context)).ToListAsync();
            return Ok(images);


        }


        [HttpDelete ("byId/{imageId}")]
        [Authorize]
        public async Task<ActionResult> deleteImage(string imageId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            var image = await _context.AppImages.FirstOrDefaultAsync(a => a.Id == imageId);
            if (image is null)
            {
                return NotFound("image not found");
            }
            _context.AppImages.Remove(image);
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

        [HttpPut("byId/{imageId}")]
        [Authorize]
        public async Task<ActionResult> nameImage(string imageId, [FromBody] StringInput input)
        {

            var NewName = input.Input;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            var image = await _context.AppImages.FirstOrDefaultAsync(a => a.Id == imageId);
            if (image is null)
            {
                return NotFound("image not found");
            }
            image.Title = NewName;
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

        [HttpPut("togglePrivatebyId/{imageId}")]
        [Authorize]
        public async Task<ActionResult> PrivateImage(string imageId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            var image = await _context.AppImages.FirstOrDefaultAsync(a => a.Id == imageId);
            if (image is null)
            {
                return NotFound("image not found");
            }
            image.Public = !image.Public;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Problem(ex.Message);
            }

            return Ok(image.Public);
        }
    }
}
