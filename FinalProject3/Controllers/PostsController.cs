﻿using FinalProject3.Data;
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
    public class PostsController(FP3Context context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly FP3Context _context = context;

        // Retrieves a list of posts along with their associated comments, votes, author, and group, filtered for the current user.
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PostDisplay>>> GetPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var posts = await _context.Post.Include(p => p.Comments).Include(p => p.Votes).Include(p => p.Author).Include(p => p.Group).ToListAsync();
            var postsDisplay = new List<PostDisplay>();
            foreach (var post in posts) {
                var postDisplay = await post.ToDisplay(userId, _context);
                    postsDisplay.Add(postDisplay);
            }
            
         
            return Ok(postsDisplay);
        }


        // Retrieves a single post by its ID, including its comments, votes, author, and group, filtered for the current user.
        [HttpGet("ById/{id}")]
        [Authorize]
        public async Task<ActionResult<PostDisplay>> GetPost(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var post = await _context.Post.Include(p => p.Comments).Include(p => p.Votes).Include(p => p.Author).Include(p => p.Group).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            var dispalyPost = await post.ToDisplay(userId, _context);
            return Ok(dispalyPost);
        }



        // Retrieves a list of posts containing a specified keyword, filtered for the current user.
        [HttpGet("ByKeyword/{KeyWord}")]
        [Authorize]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByKeyWord(string KeyWord)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var posts = await _context.Post.Where(p => p.KeyWords.Contains(KeyWord)).ToListAsync();
            var postsDisplay = new List<PostDisplay>();
            foreach (var post in posts)
            {
                var postDisplay = await post.ToDisplay(userId, _context);
                postsDisplay.Add(postDisplay);
            }

            if (postsDisplay == null)
            {
                return NotFound();
            }

            return postsDisplay;
        }
        // Retrieves a list of posts belonging to a specific group, filtered for the current user.
        [HttpGet("ByGroup/{GroupId}")]
        [Authorize]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByGroupd(string GroupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }

            var posts = await _context.Post.Include(p => p.Comments).Include(p => p.Author).Include(p => p.Group).Where(p=>p.Group != null && p.Group.Id == GroupId).ToListAsync();


            var postsDisplay = new List<PostDisplay>();
            foreach (var post in posts)
            {
                var postDisplay = await post.ToDisplay(userId, _context);
                postsDisplay.Add(postDisplay);
            }

            if (postsDisplay == null)
            {
                return NotFound();
            }

            return Ok(postsDisplay);
        }
        // Retrieves a list of posts authored by a specific author, filtered for the current user.
        [HttpGet("ByAuthor/{AuthorId}")]
        [Authorize]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByAuthor(string AuthorId, [FromBody] int page=1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            int PageSize = page*25;
            var posts = await _context.Post.Include(p => p.Comments).Include(p => p.Votes).ThenInclude(v => v.Voter).Include(p => p.Author).Include(p => p.Group).Where(p => p.Author != null && p.Author.Id == AuthorId).ToListAsync();
            var postsDisplay = new List<PostDisplay>();
            foreach (var post in posts)
            {
                var postDisplay = await post.ToDisplay(userId, _context);
                if (postDisplay != null && string.IsNullOrEmpty(postDisplay.GroupId))
                {
                postsDisplay.Add(postDisplay);
                }
               
            }
            if (postsDisplay == null)
            {
                return NotFound();
            }

            currentUser.LastActive = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
            await userManager.UpdateAsync(currentUser);

            return Ok(postsDisplay);
        }
        // Retrieves a list of posts that the current user has voted on, filtered for the current user.
        [HttpGet("ByVotedOn")]
        [Authorize]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByVotedOn()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var posts = await _context.Post.Include(p => p.Comments).Include(p => p.Votes).ThenInclude(v => v.Voter).Include(p => p.Author).Include(p => p.Group)
            .Where(p => p.Votes.Any(v => v.Voter != null && v.Voter.Id == userId)).ToListAsync();
            var postsDisplay = new List<PostDisplay>();
            foreach (var post in posts)
            {
                var postDisplay = await post.ToDisplay(userId, _context);
                postsDisplay.Add(postDisplay);
            }
            if (postsDisplay == null)
            {
                return NotFound();
            }

            return Ok(postsDisplay);
        }
        // Retrieves a list of posts that the current user has downvoted, filtered for the current user.
        [HttpGet("ByDownVote/{UserID}")]
        [Authorize]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByDownVote(string UserID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var posts = await _context.Post
            .Where(p => p.Votes.Any(v => v.Voter != null && v.Voter.Id == UserID && v.Voted < 0)).ToListAsync();
            var postsDisplay = new List<PostDisplay>();
            foreach (var post in posts)
            {
                var postDisplay = await post.ToDisplay(userId, _context);
                postsDisplay.Add(postDisplay);
            }
            if (postsDisplay == null)
            {
                return NotFound();
            }

            return Ok(postsDisplay);
        }
        // Retrieves a full post by its ID, without any filters.
        [HttpGet("FullById/{PostID}")]
        [Authorize]
        public async Task<ActionResult<Post>> GetFullPostByPostID(string PostId)
        {
            var post = await _context.Post.FindAsync(PostId);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }



        // Updates an existing post based on the provided data, ensuring no unauthorized modifications are made.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<PostDisplay>> PutPost(string id, [FromBody] PostDisplay post)
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
            if (id != post.Id)
            {
                return BadRequest();
            }

            var fullPost = await _context.Post.Include(p => p.Comments).Include(p => p.Votes).Include(p => p.Author).Include(p => p.Group).Where(p => p.Id == id).FirstOrDefaultAsync();

            if (fullPost is null)
            {
                return BadRequest();
            }

            if (post.ImageURL != fullPost.ImageURL)
            {
                fullPost.ImageURL = post.ImageURL;
            }
            if (post.Text != fullPost.Text)
            {
                fullPost.Text = post.Text;
            }
            if (post.Title != fullPost.Title)
            {
                fullPost.Title = post.Title;
            }
            if (post.Text != fullPost.Text)
            {
                fullPost.Text = post.Text;
            }
            if (post.KeyWords != fullPost.KeyWords)
            {
                fullPost.KeyWords = post.KeyWords;
            }
            if (post.Link != fullPost.Link)
            {
                fullPost.Link = post.Link;
            }
            if (post.CategoryId != fullPost.CategoryId)
            {
                fullPost.CategoryId = post.CategoryId;
            }

            _context.Entry(fullPost).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


            var postDisplay = await fullPost.ToDisplay(userId, _context);

            return Ok(postDisplay);
        }

        // Creates a new post from the provided data and associates it with the current user.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDisplay>> PostPost([FromBody] PostNew newPost)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid || userId is null)
            {
                return BadRequest(ModelState);
            }
            newPost.AuthorId = userId;
            var post = await newPost.NewPostToPost(userManager , _context);
            if (post is null)
            {
                return BadRequest("Post missing info");
            }
                _context.Post.Add(post);
            
            if (post.Group is not null)
            {
                var group = await _context.Group.FindAsync(post.Group.Id);
                group?.Posts.Add(post);

            }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (PostExists(post.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            
            var postDisplay = await post.ToDisplay(userId, _context);

            return Created("Success", postDisplay);
        }

        // Deletes a post and its associated comments and votes, ensuring the current user has proper authorization.
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var currentUser = await userManager.FindByIdAsync(userId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            var post = await _context.Post.Include(p => p.Author).Include(c => c.Votes).Include(c => c.Comments).FirstOrDefaultAsync(c => c.Id == id);
            if (post == null)
            {
                return NotFound();
            }



            post.Votes.Clear();
            post.Comments.Clear();
            _context.Post.Remove(post);

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
        // Allows the current user to vote on a post, ensuring they haven't voted already.
        [HttpPut("VoteById/{PostId}")]
        [Authorize]
        public async Task<IActionResult> VoteOnPost(string PostId, [FromBody] VoteInput vote)
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
            var fullPost = await _context.Post.Include(p => p.Comments).Include(p => p.Votes).ThenInclude(v => v.Voter).Include(p => p.Author).Include(p => p.Group).FirstOrDefaultAsync(p => p.Id == PostId);

            if (fullPost is null)
            {
                return NotFound();
            }

            var hasVoted = currentUser.votedOn.Contains(PostId);
            if (hasVoted is true)
            {
                return BadRequest("You have alredey voted");
            }
            if (vote.Vote > 0)
            {
                vote.Vote = 1;
            }
            else
            {
                vote.Vote = -1;
            }
            Votes addedVote = new();
            addedVote.CreatVote(currentUser, vote.Vote);
            currentUser.votedOn.Add(PostId);
            fullPost.Votes.Add(addedVote);
            fullPost.CalcVotes();
            _context.Update(fullPost);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(fullPost.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var postDisplay = await fullPost.ToDisplay(currentUserId, _context);
            return Ok(postDisplay);
        }

        // Allows the current user to remove their vote on a post
        [HttpPut("UnVoteById/{PostId}")]
        [Authorize]
        public async Task<IActionResult> UnVoteOnPost(string PostId)
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

            var fullPost = await _context.Post.Include(p => p.Comments).Include(p => p.Votes).ThenInclude(v => v.Voter).Include(p => p.Author).Include(p => p.Group).FirstOrDefaultAsync(p => p.Id == PostId);
            if (fullPost is null)
            {
                return NotFound("Post Not Found");
            }

            var hasVoted = currentUser.votedOn.Contains(PostId);
            if (hasVoted is true)
            {
                currentUser.votedOn.Remove(PostId);
            }
            var voteToRemove = fullPost.Votes.Where(v => v.Voter == currentUser).FirstOrDefault();
            if (voteToRemove is null)
            {
                return NotFound("Vote Not Found");
            }

            fullPost.Votes.Remove(voteToRemove);
            fullPost.CalcVotes();
            _context.Update(fullPost);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(fullPost.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var postDisplay = await fullPost.ToDisplay(currentUserId, _context);
            return Ok(postDisplay);
        }
        // Check if a post exists by its ID.
        private bool PostExists(string id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
