
namespace SmartMentor.Domain.Entiies
{
    // This class represents an interest that a user can have. It can be used to categorize users based on their interests and to recommend mentors or mentees with similar interests.
    // For example, if a user has an interest in "Data Science", they can be matched with mentors who also have an interest in "Data Science" or related fields.
    public class Interests
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserInterests> UserInterests { get; set; } = new List<UserInterests>();
    }
}
