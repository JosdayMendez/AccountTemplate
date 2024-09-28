using System.ComponentModel.DataAnnotations;

public class RegisterWithRoleVM
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "You must select at least one role.")]
    public List<string> Roles { get; set; }

    [Required(ErrorMessage = "You must agree to the terms and conditions.")]
    public bool AgreeToTerms { get; set; }
}
