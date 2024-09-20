using System.ComponentModel.DataAnnotations;

public class RecoverPasswordVM
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmNewPassword { get; set; }
}
