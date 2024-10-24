using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TennisApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Status { get; set; }  // "Member", "Coach", or "Admin"
    }
}
