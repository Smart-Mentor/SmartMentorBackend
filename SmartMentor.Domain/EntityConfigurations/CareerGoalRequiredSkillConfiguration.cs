using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class CareerGoalRequiredSkillConfiguration
     : IEntityTypeConfiguration<CareerGoalRequiredSkill>
    {
        public void Configure(EntityTypeBuilder<CareerGoalRequiredSkill> builder)
        {
            builder.ToTable("CareerGoalRequiredSkills");

            // Composite Key
            builder.HasKey(x => new { x.CareerGoalId, x.SkillId });

            builder.HasOne(x => x.CareerGoal)
                   .WithMany(x => x.RequiredSkills)
                   .HasForeignKey(x => x.CareerGoalId);

            builder.HasOne(x => x.Skill)
                   .WithMany(x => x.CareerGoalRequiredSkills)
                   .HasForeignKey(x => x.SkillId);

            builder.Property(x => x.RequiredLevel)
                   .IsRequired()
                   .HasConversion<int>(); 

            builder.Property(x => x.Priority)
                   .IsRequired(false);
        }
    }
}
