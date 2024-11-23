using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace FinalProject3.DTOs
{
    public class AppUserLogin()
    {
        [Required]
        [EmailAddress]
        public  string Email { get; set; } =string.Empty;
        [Required, PasswordPropertyText, DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        public  string Password { get; set; } = string.Empty;

        public AppUserLogin(string email, string password) : this ()
        {
            
            Email = email;
            Password = password;
        }
    }
}
