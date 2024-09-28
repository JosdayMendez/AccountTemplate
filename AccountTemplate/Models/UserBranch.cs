using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.Models
{
    public class UserBranch
    {
            [Key]
            public int Id { get; set; }

            [Required]
            public string UserId { get; set; }

            [ForeignKey("UserId")]
            public AppUser User { get; set; }

            [Required]
            public int BranchId { get; set; }

            [ForeignKey("BranchId")]
            public Branch Branch { get; set; }
    }
}
