using System.ComponentModel.DataAnnotations;

namespace FinalProject3.Models
{
    public class Interaction
    {
        [Key]
        public required string Id { get; set; }
        public string Link { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        [Required, MinLength(2)]
        public string Text { get; set; } = string.Empty;
        [Required]
        public required AppUser Author { get; set; }

        public List<Votes> Votes { get; set; } = [];

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public int TotalVotes { get; set; }

        public string Datetime { get; set; } = string.Empty;

        public List<Comment> Comments { get; set; } = new List<Comment>();
        public void calcVotes()
        {
            int UpVotes = 0;
            int DownVotes = 0;
            foreach (var vote in Votes)
            {
                if (vote.Voted == 1)
                {
                    UpVotes++;
                }
                if (vote.Voted == -1)
                {
                    DownVotes++;
                }
            }
            TotalVotes = UpVotes - DownVotes;
        }
    }
}
