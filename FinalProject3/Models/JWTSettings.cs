namespace FinalProject3.Models
{
    public class JWTSettings
    {
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }

        public static JWTSettings NewInstance()
        {
            return new JWTSettings()
            {
                SecretKey = "",
                Issuer = "",
                Audience = ""
            };
        }
    }
}
