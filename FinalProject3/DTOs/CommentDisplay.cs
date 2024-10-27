
using System.ComponentModel.DataAnnotations;

namespace FinalProject3.DTOs
{
    public class CommentDisplay
    {
        public required string Id { get; set; }
        public string Link { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public string ParentPostId { get; set; } = string.Empty;

        public bool hasVoted { get; set; } = false;

        public string ParentCommentId { get; set; } = string.Empty;
        public string Datetime { get; set; } = string.Empty;

        public List<CommentDisplay>? Comments { get; set; } = new List<CommentDisplay>();
    }
}
