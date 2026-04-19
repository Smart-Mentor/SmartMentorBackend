using System.ComponentModel.DataAnnotations.Schema;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.Entiies
{
    public class PasswordResetCodes
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; } 
        public string Code { get; set; } = null!;
        public Guid ResetToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsUsed { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }

}