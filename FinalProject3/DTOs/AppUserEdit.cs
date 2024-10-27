
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FinalProject3.DTOs
{
    public class AppUserEdit
    {
        public string id { get; set; } = string.Empty;

        public  string userName { get; set; } = string.Empty;

        public  string oldPassword { get; set; } = string.Empty;

        public  string newPassword { get; set; } = string.Empty;
        public  string bio { get; set; } = string.Empty;

        public  string prefix { get; set; } = string.Empty;
        public bool hideEmail { get; set; } = false;
        public bool hideName { get; set; } = false;
        public bool hideBlocked { get; set; } = false;
        public string banerImageURL { get; set; } = string.Empty;
        public  string first_Name { get; set; } = string.Empty;

        public  string last_Name { get; set; } = string.Empty;

        public  string pronouns { get; set; } = string.Empty;

        public  string imageURL { get; set; } = string.Empty;

        public  string permissionLevel { get; set; } = string.Empty;
    }
}
