using FinalProject3.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject3.DTOs
{
    public class SocialGroupEdit
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string NewAdminEmail { get; set; } = string.Empty;
        public string GroupRules { get; set; } = string.Empty;

        public string ImageURL { get; set; } = string.Empty;
        public string BanerImageURL { get; set; } = string.Empty;
    }
}


