using FinalProject3.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject3.DTOs
{
    public class AppUserDisplay
    {
        public string Id { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string First_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public bool HideEmail { get; set; } = false;
        public bool HideName { get; set; } = false;
        public bool HideBlocked { get; set; } = false;
        public string BanerImageURL { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public string Pronouns { get; set; } = string.Empty;
        public bool Following { get; set; } = false;
        public bool Blocked { get; set; } = false;
        public bool BlockedYou { get; set; } = false;
        public string LastActive {  get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public List<string> votedOn { get; set; } = [];


    }
}
