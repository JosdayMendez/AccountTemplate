using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountTemplate.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AppUser")]
        public string UserId { get; set; }
        public AppUser AppUser { get; set; }

        [Required(ErrorMessage = "Business name is required.")]
        public string BusinessName { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string PrimaryEmail { get; set; }

        [EmailAddress]
        public string SecondaryEmail { get; set; }
    }
}
