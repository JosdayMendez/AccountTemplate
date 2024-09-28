using AccountTemplate.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class ProfileVM
    {
        public string? BusinessName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid primary email address.")]
        public string PrimaryEmail { get; set; }

        [EmailAddress(ErrorMessage = "Invalid secondary email address.")]
        public string? SecondaryEmail { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        public string? Role { get; set; }

        public List<string>? Branches { get; set; } 
        public string? SelectedBranch { get; set; }
    }
}
