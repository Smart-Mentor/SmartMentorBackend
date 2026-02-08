using Microsoft.AspNetCore.Identity;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Persistence.Identity
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CareerGoalId { get; set; }// Foreign key to CareerGoal
        public CareerGoal CareerGoal { get; set; }
        public bool IsProfileCompleted { get; set; }// This property indicates whether the user has completed their profile. It can be used to prompt users to complete their profile and to restrict access to certain features until the profile is completed.
    }
}
