using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccountTemplate.Models
{
    public class Credit
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public decimal Balance { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public virtual AppUser User { get; set; }
    }
}
