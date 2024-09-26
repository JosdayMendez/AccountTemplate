using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class BranchVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "WhatsApp")]
        public string WhatsApp { get; set; }

        [Required]
        [Display(Name = "Geolocation")]
        public string Geolocation { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
