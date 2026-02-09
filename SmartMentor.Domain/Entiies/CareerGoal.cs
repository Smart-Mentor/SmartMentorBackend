

// This class represents a career goal that a user can have. It can be used to categorize users based on their career goals and to recommend mentors or mentees with similar career goals.
// For example, if a user has a career goal of "Become a Software Engineer", they
// can be matched with mentors who have a similar career goal or who have achieved that career goal.
namespace SmartMentor.Domain.Entiies
{
    public class CareerGoal
    {
        public  int  Id { get; set; }
        public string  Name { get; set; }
        public string? Description { get; set; }
    }
}
