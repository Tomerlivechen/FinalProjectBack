namespace FinalProject3.DTOs
{
    public class SendGuid
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Val { get; set; } = Guid.NewGuid().ToString();

    }
}
