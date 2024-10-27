namespace FinalProject3.Models
{
    public class Chat
    {
        public string Id { get; set; } = string.Empty;
        public List<AppUser> Users { get; set; } = new List<AppUser>();

        public string User1Id { get; set; } = string.Empty;
        public string User1Name { get; set; } = string.Empty;
        public string User2Id { get; set; } = string.Empty;
        public string User2Name { get; set; } = string.Empty;

        public List<Message> messages { get; set; } = new List<Message>();
    }
}
