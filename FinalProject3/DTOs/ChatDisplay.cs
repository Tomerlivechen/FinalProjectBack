using FinalProject3.Models;

namespace FinalProject3.DTOs
{
    public class ChatDisplay
    {
        public string Id { get; set; } = string.Empty;

        public string User1Id { get; set; } = string.Empty;
        public string User1Name { get; set; } = string.Empty;
        public string User2Id { get; set; } = string.Empty;
        public string User2Name { get; set; } = string.Empty;

        public List<MessageDisplay> messages { get; set; } = new List<MessageDisplay>();
    }
}
