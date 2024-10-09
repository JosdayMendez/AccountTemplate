using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class CreditVM
    {
        public string UserId { get; set; }

        [Display(Name = "Current Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Amount to Purchase")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid amount of credits.")]
        public decimal PurchaseAmount { get; set; }

        [Display(Name = "Amount to Spend")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid amount of credits.")]
        public decimal SpendAmount { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
