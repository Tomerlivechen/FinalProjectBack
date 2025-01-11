using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Mapping;
using FinalProject3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalProject3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(FP3Context context, UserManager<AppUser> userManager) : ControllerBase
    {


        private readonly FP3Context _context = context;
        // Gets a list of comments for a specific post identified by PostID, including the comments' authors and replies.

        [HttpGet("ByPPostId/{PostID}")]
        [Authorize]
        public async Task<ActionResult<List<CommentDisplay>>> GetCommentByPPostID(string PostID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var comments = await _context.Comment.Include(c=>c.Author).Include(c=>c.Comments).ThenInclude(cc=>cc.Author).Where(c => c.ParentPost != null && c.ParentPost.Id == PostID).ToListAsync();

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
        // Gets a list of comments for a specific comment identified by CommentID, including the comments' authors and replies.

        [HttpGet("ByPCommentId/{CommentID}")]
        [Authorize]
        public async Task<ActionResult<List<CommentDisplay>>> GetCommentByPCommentID(string CommentID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var comments = await _context.Comment.Include(c => c.Author).Include(c => c.Comments).ThenInclude(cc => cc.Author).Where(c => c.ParentComment != null && c.ParentComment.Id == CommentID).ToListAsync();

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
        // Gets a specific comment identified by CommentID, including its author and replies.

        [HttpGet("ByCommentId/{CommentID}")]
        [Authorize]
        public async Task<ActionResult<CommentDisplay>> GetCommentByCommentID(string CommentID)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var comment = await _context.Comment.Include(c => c.Author).Include(c => c.Comments).ThenInclude(cc => cc.Author).Where(c=>c.Id==CommentID).FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound();
            }



            var commentDisplay = await comment.ToDisplayAsync(userId, _context);

            return commentDisplay;
        }
        // Retrieves the full details of a specific comment identified by CommentID.

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
        // Creates a new comment. Includes validation and adds the comment to a post or another comment if applicable.

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
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            comment.AuthorId = userId;

                var newComment = await comment.NewCommentToComment(userManager);
            if (newComment.Author is null)
            {
                newComment.Author = currentUser;
            }
                _context.Comment.Add(newComment);
            Interaction? parent = null;
            bool flag = false;
            if (!string.IsNullOrEmpty(comment.ParentPostId))
            {
                parent = await _context.Post.Include(p => p.Author).Where(p => p.Id == comment.ParentPostId).FirstOrDefaultAsync();
            }
            else if (!string.IsNullOrEmpty(comment.ParentCommentId))
            {
                parent = await _context.Comment.Include(c => c.Author).Where(c => c.Id == comment.ParentCommentId).FirstOrDefaultAsync();
                flag = true;

            }
            if (parent is null)
            {
                return BadRequest(!string.IsNullOrEmpty(comment.ParentPostId) ? "Post not found" : "Comment not found");
            }
            parent.Comments.Add(newComment);
            if (parent.Author.Id == userId)
            {
                flag = true;
            }
            if (!flag)
            {
                var Notified = parent.Author;
                var newNotification = new NotificationNew
                {
                    NotifierId = userId,
                    NotifiedId = Notified.Id,
                    Type = "Comment",
                    ReferenceId = parent.Id
                };
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
        // Updates an existing comment identified by its ID, including validation and concurrency handling.

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
            var fullComment = await _context.Comment.Include(p => p.Author).Include(p => p.ParentComment).Include(p => p.ParentPost).Include(c => c.Comments).ThenInclude(cc => cc.Author).Where(p => p.Id == id).FirstOrDefaultAsync();
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
        // Deletes a comment identified by its ID, clearing associated votes before removal.

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(string id)
        {
            var comment = await _context.Comment.FirstOrDefaultAsync(c => c.Id == id);
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
        // Allows a user to vote on a specific comment identified by commentId, validating the user's action and updating the vote count.

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
            var fullComment = await _context.Comment.Include(c => c.Author).Include(c => c.Comments).ThenInclude(cc => cc.Author).Where(p => p.Id == commentId).FirstOrDefaultAsync();
            if (fullComment is null)
            {
                return NotFound();
            }
            var hasVoted = currentUser.votedOn.Contains(commentId);
            if (hasVoted is true)
            {
                return BadRequest("You have alredey voted");
            }
            Votes addedVote = new();
            addedVote.CreatVote(currentUser, Vote);
            currentUser.votedOn.Add(commentId);
            fullComment.Votes.Add(addedVote);
            fullComment.CalcVotes();
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

        // Allows a user to remove their vote from a specific comment identified by commentId.

        [HttpPut("UnVoteById/{commentId}")]
        [Authorize]
        public async Task<IActionResult> UnVoteOnPost(string commentId)
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var fullComment = await _context.Comment.Include(c => c.Votes).ThenInclude(v => v.Voter).Include(c => c.Author).Include(c => c.Comments).ThenInclude(cc => cc.Author).Where(c => c.Id == commentId).FirstOrDefaultAsync();
            if (fullComment is null)
            {
                return NotFound("Comment Not Found");
            }

            var hasVoted = currentUser.votedOn.Contains(commentId);
            if (hasVoted is true)
            {
                currentUser.votedOn.Remove(commentId);
            }
            var voteToRemove = fullComment.Votes.Where(v => v.Voter == currentUser).FirstOrDefault();
            if (voteToRemove is null)
            {
                return NotFound("Vote Not Found");
            }

            fullComment.Votes.Remove(voteToRemove);
            fullComment.CalcVotes();
            _context.Update(fullComment);
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
        // Checks if a comment exists in the database using its ID.
        private bool CommentExists(string id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
