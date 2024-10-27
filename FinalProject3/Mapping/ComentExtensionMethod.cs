using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

namespace FinalProject3.Mapping
{
    public static class ComentExtensionMethod
    {
        public static async Task<CommentDisplay> ToDisplayAsync(this Comment comment, string userID, FP3Context _context)
        {

            CommentDisplay setcomment =  new CommentDisplay()
            {
                Id = comment.Id,
                Text = comment.Text,
                Link = comment.Link,
                ImageURL = comment.ImageURL,
                AuthorName = comment.Author.UserName,
                AuthorId = comment.Author.Id,
                TotalVotes = comment.TotalVotes,
                Datetime = comment.Datetime,
            };
            if (comment.ParentComment is not null)
            {
                setcomment.ParentCommentId = comment.ParentComment.Id;
            }
            if (comment.ParentPost is not null)
            {
                setcomment.ParentCommentId = comment.ParentPost.Id;
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userID);
            if (currentUser is not null)
            {
                setcomment.hasVoted = currentUser.votedOn.Contains(setcomment.Id);
            }
            setcomment.Comments = new List<CommentDisplay>();
            foreach (Comment com in comment.Comments)
            {
                var displaycom = await com.ToDisplayAsync(userID, _context);
                if (displaycom is not null)
                {
                    setcomment.Comments.Add(displaycom);
                }
            }
            return setcomment;
        }
        public async static Task<Comment> NewCommentToComment(this CommentNew NewComment, UserManager<AppUser> userManager)
        {
            AppUser? appUser = await userManager.FindByIdAsync(NewComment.AuthorId);
            if (appUser is null)
            {
                throw new InvalidOperationException("now Author");
            }
                Comment comment = new Comment()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = NewComment.Text,
                    Link = NewComment.Link,
                    ImageURL = NewComment.ImageURL,
                    Author = appUser,
                    Datetime = NewComment.Datetime,

                };
                return comment;
            }
            
        }

    }
