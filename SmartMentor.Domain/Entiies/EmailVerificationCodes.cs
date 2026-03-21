using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.Entiies
{
    public class EmailVerificationCodes
    {
        public int Guid { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        // foreign key
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid VerficationToken { get; set; }
    }
}
