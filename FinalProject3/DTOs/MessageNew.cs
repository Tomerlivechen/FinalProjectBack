namespace FinalProject3.DTOs
{
    public class MessageNew
    {
        

        public string ChatId { get; set; } = string.Empty;

        public string message { get; set; } = string.Empty;

        public string Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
    }
}
