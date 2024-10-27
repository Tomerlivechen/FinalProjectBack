using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using APIClassLibraryDAL.Models;

namespace FinalProject3.DTOs
{
    public class AppUserRegister
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string UserName { get; set; }

        [Required, PasswordPropertyText]
        public required string Password { get; set; }

        [Required]
        public required string Prefix { get; set; } 
        [Required]
        public required string First_Name { get; set; } 
        [Required]
        public required string Last_Name { get; set; } 
        [Required]
        public required string Pronouns { get; set; } 
        [Required]
        public required string ImageURL { get; set; }
        [Required]
        public required string PermissionLevel { get; set; }
    }
}
