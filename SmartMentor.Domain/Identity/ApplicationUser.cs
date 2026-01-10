using Microsoft.AspNetCore.Identity;

namespace SmartMentor.Persistence.Identity
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
