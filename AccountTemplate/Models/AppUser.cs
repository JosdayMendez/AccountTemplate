using AccountTemplate.ViewModels;
using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string Name { get; set; }

}