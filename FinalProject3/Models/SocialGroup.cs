using System.ComponentModel.DataAnnotations;

namespace FinalProject3.Models
{
    public class SocialGroup
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string GroupCreatorId { get; set; } = string.Empty;
        public string GroupRules { get; set; } = string.Empty;

        public AppUser? GroupCreator { get; set; }


        public string AdminId { get; set; } = string.Empty;
        public AppUser? groupAdmin { get; set; } 

        public List<AppUser> Members { get; set; } = new List<AppUser>();

        public List<Post> Posts { get; set; } = new List<Post>();

        public string ImageURL { get; set; } = string.Empty;
        public string BanerImageURL { get; set; } = string.Empty;

    }
}
