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

namespace FinalProject3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController(FP3Context context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly FP3Context _context = context;

        // GET: api/Posts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PostDisplay>>> GetPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var posts = await _context.Post.Include(p => p.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).Include(p => p.Votes).Include(p => p.Author).Include(p => p.Group).ToListAsync();

            var postsDisplay = new List<PostDisplay>();
            foreach (var post in posts) {
                var postDisplay = await post.ToDisplay(userId, _context);
                    postsDisplay.Add(postDisplay);
            }
            
         
            return Ok(postsDisplay);
        }


        // GET: api/Posts/5
        [HttpGet("ById/{id}")]
        [Authorize]
        public async Task<ActionResult<PostDisplay>> GetPost(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var post = await _context.Post.Include(p => p.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).Include(p => p.Votes).Include(p => p.Author).Include(p => p.Group).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            var dispalyPost = await post.ToDisplay(userId, _context);
            return Ok(dispalyPost);
        }



        // GET: api/Posts/5
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

        [HttpGet("ByGroup/{GroupId}")]
        [Authorize]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByGroupd(string GroupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }

            var posts = await _context.Post.Include(p => p.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).Include(p => p.Author).Include(p => p.Group).Where(p=>p.Group != null && p.Group.Id == GroupId).ToListAsync();


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

        [HttpGet("ByAuthor/{AuthorId}")]
        [Authorize]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByAuthor(string AuthorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var posts = await _context.Post.Include(p => p.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).ThenInclude(c => c.Comments).Include(p => p.Votes).ThenInclude(v => v.Voter).Include(p => p.Author).Include(p => p.Group).Where(p => p.Author != null && p.Author.Id == AuthorId).ToListAsync();
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

            return Ok(postsDisplay);
        }

        [HttpGet("ByUpVote/{UserID}")]
        public async Task<ActionResult<List<PostDisplay>>> GetPostByUpVote(string UserID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var posts = await _context.Post
            .Where(p => p.Votes.Any(v => v.Voter != null && v.Voter.Id == UserID && v.Voted > 0)).ToListAsync();
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



        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(string id)
        {
            var post = await _context.Post.Include(c => c.Votes).FirstOrDefaultAsync(c => c.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            post.Votes.Clear();
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
            var fullPost = await _context.Post.Include(p => p.Votes).Include(p => p.Author).Where(p => p.Id== PostId).FirstOrDefaultAsync();
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
            fullPost.calcVotes();
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
        private bool PostExists(string id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
