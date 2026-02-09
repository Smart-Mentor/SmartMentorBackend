using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.Entiies
{
    public class UserInterests
    {
        public Guid UserId { get; set; }// Foreign key to ApplicationUser
        public int InterestId { get; set; }// Foreign key to Interests
        public Interests Interest { get; set; }
        public ApplicationUser User { get; set; }
    }
}