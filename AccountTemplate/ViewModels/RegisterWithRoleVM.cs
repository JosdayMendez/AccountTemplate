using System.ComponentModel.DataAnnotations;

public class RegisterWithRoleVM
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "You must confirm the password.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "You must select at least one role.")]
    public List<string> Roles { get; set; }

    [Required(ErrorMessage = "You must agree to the terms and conditions.")]
    public bool AgreeToTerms { get; set; }
}
