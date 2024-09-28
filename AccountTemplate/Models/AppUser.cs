using AccountTemplate.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class AppUser : IdentityUser
{
    public string Name { get; set; }

    public ICollection<UserBranch> UserBranches { get; set; }

}
