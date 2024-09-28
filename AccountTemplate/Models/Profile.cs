using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Profile
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("AppUser")]
    public string UserId { get; set; }
    public AppUser AppUser { get; set; }

    public string BusinessName { get; set; } 

    [Phone(ErrorMessage = "Invalid phone number.")]
    public string Phone { get; set; }

    [EmailAddress]
    public string PrimaryEmail { get; set; } 

    [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
    public string UserName { get; set; } 

    [EmailAddress]
    public string SecondaryEmail { get; set; }

    [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
    public string Role { get; set; } = "N/A";
}
