using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace FinalProject3.DTOs
{
    public class AppUserRegister
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "User name must be between 2 and 25 characters.")]
        public required string UserName { get; set; }

        [Required, PasswordPropertyText]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        public required string Password { get; set; }


        [StringLength(5, MinimumLength = 2, ErrorMessage = "Prefix must be between 2 and 5 characters.")]
        public required string Prefix { get; set; } 
        [Required]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 25 characters.")]
        public required string First_Name { get; set; } 
        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 30 characters.")]
        public required string Last_Name { get; set; } 

        [StringLength(10, MinimumLength = 2, ErrorMessage = "Pronouns must be between 2 and 10 characters.")]
        public required string Pronouns { get; set; } 
        [Required]
        public required string ImageURL { get; set; }
        [Required]
        public required string PermissionLevel { get; set; }
    }
}
