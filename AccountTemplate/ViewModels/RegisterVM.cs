using System.ComponentModel.DataAnnotations;

public class RegisterVM
{
    [Required(ErrorMessage = "Id is required.")]
    public string Id { get; set; } 

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "You must agree to the terms.")]
    public bool AgreeToTerms { get; set; }
}
