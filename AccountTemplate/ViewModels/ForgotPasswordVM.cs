using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
