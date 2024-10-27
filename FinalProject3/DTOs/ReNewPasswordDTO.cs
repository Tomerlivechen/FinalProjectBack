namespace FinalProject3.DTOs
{
    public class ReNewPasswordDTO
    {
        public string userEmail { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string newPassword { get; set; } = string.Empty;
    }
}
