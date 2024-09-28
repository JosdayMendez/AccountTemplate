using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class BranchVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The branch name is required")]
        [StringLength(50, ErrorMessage = "The branch name cannot exceed 50 characters")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "The address is required")]
        [StringLength(100, ErrorMessage = "The address cannot exceed 100 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "The phone number is required")]
        [StringLength(20, ErrorMessage = "The phone number cannot exceed 20 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "WhatsApp is required")]
        [StringLength(20, ErrorMessage = "WhatsApp cannot exceed 20 characters")]
        public string WhatsApp { get; set; }

        [Required(ErrorMessage = "The Google Maps link is required")]
        [StringLength(200, ErrorMessage = "The Google Maps link cannot exceed 200 characters")]
        public string GoogleMapsLink { get; set; }

        [Required(ErrorMessage = "The email is required")]
        [EmailAddress(ErrorMessage = "The email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The description is required")]
        [StringLength(200, ErrorMessage = "The description cannot exceed 200 characters")]
        public string Description { get; set; }
    }
}
