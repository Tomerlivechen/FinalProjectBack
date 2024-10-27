namespace FinalProject3.Models
{
    public class Votes
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public AppUser? Voter { get; set; }

        public int Voted {  get; set; }    
    }
}
