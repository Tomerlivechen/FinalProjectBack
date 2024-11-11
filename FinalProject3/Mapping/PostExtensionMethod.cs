using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace FinalProject3.Mapping
{
    public static class PostExtensionMethod 
    {
        public static async Task<PostDisplay> ToDisplay (this Post post, string userID , FP3Context _context)
        {
            var pAutherName = post.Author.UserName;
            var setPost = new PostDisplay()
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                CategoryId = post.CategoryId,
                KeyWords = post.KeyWords,
                Link = post.Link,
                ImageURL = post.ImageURL,
                AuthorId = post.Author.Id,
                TotalVotes = post.TotalVotes,
                Datetime = post.Datetime,

            };
            if (pAutherName is not null)
            {
                setPost.AuthorName = pAutherName;
            }
            var currentUser = await _context.Users.
                FirstOrDefaultAsync(u => u.Id == userID);
            if (post.Group != null)
            {
                setPost.GroupId = post.Group.Id;
            }

            if (currentUser is not null)
            {
                setPost.hasVoted = currentUser.votedOn.Contains(post.Id);
            }

            setPost.Comments = [];
            foreach (Comment comment in post.Comments)
            {
                var tempComment = await _context.Comment.Include(c => c.Author).Include(c => c.Comments).ThenInclude(cc => cc.Author).Where(p => p.Id == comment.Id).FirstOrDefaultAsync();
                if (tempComment is not null)
                {
                    var displaycom = await tempComment.ToDisplayAsync(userID, _context);
                    if (displaycom is not null)
                    {
                        setPost.Comments.Add(displaycom);
                    }
                }
            }
            return setPost;
        }

        public async static Task<Post?> NewPostToPost(this PostNew Newpost, UserManager<AppUser> userManager, FP3Context context)
        {
            var pAuther = await userManager.FindByIdAsync(Newpost.AuthorId);
            char[] delimiters = [',', ';', '|',' '];
            if (pAuther is null)
            {
                return default;
            }
                var setPost = new Post()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Newpost.Title,
                    Text = Newpost.Text,
                    CategoryId = Newpost.CategoryId,
                    Author = pAuther,
                    Link = Newpost.Link,
                    ImageURL = Newpost.ImageURL,
                    Datetime = Newpost.Datetime,
                    Group = await context.Group.FindAsync(Newpost.Group)
                };

                if (Newpost.KeyWords.Trim().Length > 1)
                {
                    var key = Newpost.KeyWords.Split(delimiters).Select(x => x.Trim()).ToList();
                    List<string> keyword = [];
                    foreach (var word in key)
                    {
                        if (word.Length > 1)
                        {
                            keyword.Add(word.Capitelize());
                        }
                    }
                    setPost.KeyWords = keyword;
                }
                var user = await userManager.FindByIdAsync(Newpost.AuthorId);
                user?.Posts.Add(setPost);
                return setPost;
            
        }
    }
}
