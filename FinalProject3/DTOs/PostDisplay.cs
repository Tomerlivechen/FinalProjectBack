using FinalProject3.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject3.DTOs
{
    public class PostDisplay
    {
        public required string Id { get; set; }
        public string Link { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;

        public int TotalVotes { get; set; }
        public bool hasVoted { get; set; }

        public required string Title { get; set; }
        public int CategoryId { get; set; }
        public string GroupId { get; set; } = string.Empty;

        public List<string> KeyWords { get; set; } = [];
        public string Datetime { get; set; } = string.Empty;

        public List<CommentDisplay>? Comments { get; set; } = [];
    }
}
