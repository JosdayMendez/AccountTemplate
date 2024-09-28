using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountTemplate.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(50, ErrorMessage = "Branch name cannot exceed 50 characters")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "WhatsApp is required")]
        [StringLength(20, ErrorMessage = "WhatsApp cannot exceed 20 characters")]
        public string WhatsApp { get; set; }

        [Required(ErrorMessage = "Google Maps link is required")]
        [StringLength(200, ErrorMessage = "Google Maps link cannot exceed 200 characters")]
        public string GoogleMapsLink { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; }

        public string UserId { get; set; }

        public ICollection<UserBranch> UserBranches { get; set; }
    }
}
