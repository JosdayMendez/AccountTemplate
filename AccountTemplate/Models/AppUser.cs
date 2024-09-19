using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed {1} characters.")]
        public string? Name { get; set; }


    }
}
