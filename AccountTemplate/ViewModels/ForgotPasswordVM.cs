using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
