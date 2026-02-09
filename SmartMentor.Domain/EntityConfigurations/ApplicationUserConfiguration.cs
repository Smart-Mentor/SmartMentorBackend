using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.HasOne(u => u.CareerGoal)
                   .WithMany() // Assuming CareerGoal does not have a collection of ApplicationUsers
                   .HasForeignKey(u => u.CareerGoalId)
                   .OnDelete(DeleteBehavior.SetNull); // Set to null if the related CareerGoal is deleted
            builder.Property(u => u.IsProfileCompleted).IsRequired();
        }
    }
}