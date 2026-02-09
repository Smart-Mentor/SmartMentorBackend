using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
           builder.HasKey(s => s.Id);
           builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
           builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
        }
    }



}
