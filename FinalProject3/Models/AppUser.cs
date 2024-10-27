

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FinalProject3.Models
{
    public class AppUser : IdentityUser
    {

        public string Prefix { get; set; } = string.Empty;
        public string First_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;

        public string Pronouns { get; set; } = string.Empty;

        public string ImageURL { get; set; } = string.Empty;
        public string BanerImageURL { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ImageAlt { get; set; } = string.Empty;



        public bool HideEmail { get; set; } = false;
        public bool HideName { get; set; } = false;
        public bool HideBlocked { get; set; } = false;

        [Required]
        public required string PermissionLevel { get; set; }

        public List<AppUser> Following { get; set; } = [];
        public List<AppUser> Blocked { get; set; } = [];

        public List<string> FollowingId { get; set; } = [];
        public List<string> BlockedId { get; set; } = [];

        public List<Post> Posts { get; set; } = [];

        public List<SocialGroup> SocialGroups { get; set; } = [];

        public List<string> votedOn { get; set; } = [];

        public int VoteScore { get; set; } = 0;

        public List<Chat> Chats { get; set; } = new List<Chat>();
        public List<string> ChatsId { get; set; } = new List<string>();

        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public string LastActive { get; set; } = string.Empty;

    }
}
