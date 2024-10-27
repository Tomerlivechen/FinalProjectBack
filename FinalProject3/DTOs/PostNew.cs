using FinalProject3.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject3.DTOs
{
    public class PostNew
    {
        [Key]
        public required string Id { get; set; }
        [Required, MinLength(2), MaxLength(55)]
        public required string Title { get; set; }
        public string Link { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        [Required, MinLength(2)]
        public string Text { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string? Group { get; set; }

        public string KeyWords { get; set; } = string.Empty;
        public string Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");



    }
}
