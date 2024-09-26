using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.ViewModels
{
    public class CreateRoleVM
    {
        [Required]
        public string roleName { get; set; }
    }
}
