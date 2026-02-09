
namespace SmartMentor.Domain.Entiies
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public ICollection<UserSkills> UserSkills { get; set; } = new List<UserSkills>();
    }
}
