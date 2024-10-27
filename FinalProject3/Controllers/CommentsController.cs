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
using System.Security.Claims;
using System.Xml.Linq;

namespace FinalProject3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(FP3Context context, UserManager<AppUser> userManager) : ControllerBase
    {


        private readonly FP3Context _context = context;
        [HttpGet("ByPPostId/{PostID}")]
        [Authorize]
        public async Task<ActionResult<List<CommentDisplay>>> GetCommentByPPostID(string PostID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var comments = await _context.Comment.Where(c => c.ParentPost != null && c.ParentPost.Id == PostID).ToListAsync();

            var commentsDisplay = new List<CommentDisplay>();
            foreach (var comment in comments)
            {
                var commentDisplay = await comment.ToDisplayAsync(userId, _context);
                commentsDisplay.Add(commentDisplay);
            }
            if (commentsDisplay == null)
            {
                return NotFound();
            }


            return Ok(commentsDisplay);
        }

        [HttpGet("ByPCommentId/{CommentID}")]
        [Authorize]
        public async Task<ActionResult<List<CommentDisplay>>> GetCommentByPCommentID(string CommentID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var comments = await _context.Comment.Where(c => c.ParentComment != null && c.ParentComment.Id == CommentID).ToListAsync();

            var commentsDisplay = new List<CommentDisplay>();
            foreach (var comment in comments)
            {
                var commentDisplay = await comment.ToDisplayAsync(userId, _context);
                commentsDisplay.Add(commentDisplay);
            }
            if (commentsDisplay == null)
            {
                return NotFound();
            }


            return Ok(commentsDisplay);
        }

        [HttpGet("ByCommentId/{CommentID}")]
        [Authorize]
        public async Task<ActionResult<CommentDisplay>> GetCommentByCommentID(string CommentID)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var comment = await _context.Comment.FindAsync(CommentID);

            if (comment == null)
            {
                return NotFound();
            }



            var commentDisplay = await comment.ToDisplayAsync(userId, _context);

            return commentDisplay;
        }

        [HttpGet("ById/{CommentID}")]
        public async Task<ActionResult<Comment>> GetFullCommentByCommentID(string CommentID)
        {
            var comment = await _context.Comment.FindAsync(CommentID);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDisplay>> PostComment([FromBody] CommentNew comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (userId is null)
            {
                return BadRequest("user not found");
            }
            comment.AuthorId = userId;
                var newComment = await comment.NewCommentToComment(userManager);
                _context.Comment.Add(newComment);
            Interaction? parent;
            bool flag = false;
            if (!string.IsNullOrEmpty(comment.ParentPostId))
            {
                parent = await _context.Post.FindAsync(comment.ParentPostId);
            }
            else
            {
                parent = await _context.Comment.FindAsync(comment.ParentCommentId);
                flag = true;

            }
            if (parent is null)
            {
                return BadRequest(!string.IsNullOrEmpty(comment.ParentPostId) ? "Post not found" : "Comment not found");
            }
            parent.Comments.Add(newComment);
            if (!flag)
            {
                var Notified = parent.Author;
                var newNotification = new NotificationNew();
                newNotification.NotifierId = userId;
                newNotification.NotifiedId = Notified.Id;
                newNotification.Type = "Comment";
                newNotification.ReferenceId = parent.Id;
                Notification notification = await newNotification.AddNotification(_context);
                await _context.Notification.AddAsync(notification);
                Notified.Notifications.Add(notification);
            }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return  Problem(ex.Message);
                }

            var commentDisplay = await newComment.ToDisplayAsync(userId, _context);
            return Created("Success", commentDisplay);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutComment(string id, [FromBody] CommentDisplay comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != comment.Id)
            {
                return BadRequest("Comment ID mismatch.");
            }
            var fullComment = await _context.Comment.Include(p => p.Author).Include(p => p.ParentComment).Include(p => p.ParentPost).Where(p => p.Id == id).FirstOrDefaultAsync();
            if (fullComment is null)
            {
                return BadRequest();
            }
            if (comment.ImageURL != fullComment.ImageURL)
            {
                fullComment.ImageURL = comment.ImageURL;
            }
            if (comment.Text != fullComment.Text)
            {
                fullComment.Text = comment.Text;
            }
            if (comment.Text != fullComment.Text)
            {
                fullComment.Text = comment.Text;
            }
            if (comment.Link != fullComment.Link)
            {
                fullComment.Link = comment.Link;
            }
            _context.Entry(fullComment).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            var commentDisplay = await fullComment.ToDisplayAsync(userId, _context);

            return Ok(commentDisplay);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(string id)
        {
            var comment = await _context.Comment.Include(c => c.Votes).FirstOrDefaultAsync(c => c.Id == id);
            if (comment is null)
            {
                return NotFound();
            }
            comment.Votes.Clear();
            _context.Comment.Remove(comment);

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

        [HttpPut("VoteById/{commentId}")]
        [Authorize]
        public async Task<IActionResult> VoteOnComment(string commentId, [FromBody] VoteInput vote)
        {
            var Vote = vote.Vote;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var fullComment = await _context.Comment.Include(p => p.Votes).ThenInclude(v => v.Voter).Where(p => p.Id == commentId).FirstOrDefaultAsync();
            if (fullComment is null)
            {
                return NotFound();
            }
            var hasVoted = currentUser.votedOn.Contains(commentId);
            if (hasVoted is true)
            {
                return BadRequest("You have alredey voted");
            }
            Votes addedVote = new Votes();
            addedVote.CreatVote(currentUser, Vote);
            currentUser.votedOn.Add(commentId);
            fullComment.Votes.Add(addedVote);
            fullComment.calcVotes();
            _context.Update(fullComment);
            if (Vote > 0)
            {
                Vote = 1;
            }
            else
            {
                Vote = -1;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(fullComment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var commentDisplay = await fullComment.ToDisplayAsync(currentUserId, _context);
            return Ok(commentDisplay);
        }

        private bool CommentExists(string id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
