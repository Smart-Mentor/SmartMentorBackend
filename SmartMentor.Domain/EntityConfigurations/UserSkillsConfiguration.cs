using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{

    public class UserSkillsConfiguration : IEntityTypeConfiguration<UserSkills>
    {
        public void Configure(EntityTypeBuilder<UserSkills> builder)
        {
            builder.HasKey(us => new { us.UserId, us.SkillId }); // Composite key of UserId and SkillId
            builder.HasOne(us => us.User)// this means UserId is a foreign key to ApplicationUser table
                     .WithMany(u => u.UserSkills)
                   .HasForeignKey(us => us.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(us => us.Skill)// this means SkillId is a foreign key to Skill table
                     .WithMany(s => s.UserSkills)
                   .HasForeignKey(us => us.SkillId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(us => us.SkillLevel).IsRequired().HasMaxLength(50);
        }
    }
}