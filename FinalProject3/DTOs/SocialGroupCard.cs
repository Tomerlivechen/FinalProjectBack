namespace FinalProject3.DTOs
{
    public class SocialGroupCard
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string GroupRules { get; set; } = string.Empty;
        public string BanerImageURL { get; set; } = string.Empty;
        public AppUserDisplay Admin { get; set; } = new AppUserDisplay();

        public bool IsMemember { get; set; }

    }
}
