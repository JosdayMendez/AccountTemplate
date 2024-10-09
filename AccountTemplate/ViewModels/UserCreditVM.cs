using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class UserCreditVM
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [Display(Name = "Current Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }
    }
}
