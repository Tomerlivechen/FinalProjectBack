using System.ComponentModel.DataAnnotations;

namespace FinalProject3.Models
{
    public class Post : Interaction
    {
        [Required, MinLength(2), MaxLength(55)]
        public required string Title { get; set; }
        public int CategoryId { get; set; }

        public SocialGroup? Group { get; set; }

        public List<string> KeyWords { get; set; } = [];

        

        

    }
}
