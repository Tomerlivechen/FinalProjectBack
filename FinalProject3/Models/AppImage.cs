namespace FinalProject3.Models
{
    public class AppImage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public AppUser? user { get; set; }
        public string Url { get; set; } = string.Empty;

        public string Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");

        public string Title { get; set; } = string.Empty;

        public bool Public { get; set; } = false;


    }
}
