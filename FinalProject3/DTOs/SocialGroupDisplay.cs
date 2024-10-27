using FinalProject3.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject3.DTOs
{
    public class SocialGroupDisplay
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GroupRules { get; set; } = string.Empty;

        public string ImageURL { get; set; } = string.Empty;
        public string BanerImageURL { get; set; } = string.Empty;

        public bool IsMemember { get; set; }

        public string GroupCreatorId { get; set; } = string.Empty;

        public string AdminId { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;


    }
}
