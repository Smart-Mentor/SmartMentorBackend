using SmartMentor.Domain.Entiies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Persistence.Identity;
namespace SmartMentor.Domain.EntityConfigurations
{
    public class CareerGoalConfiguration : IEntityTypeConfiguration<CareerGoal>
    {
        public void Configure(EntityTypeBuilder<CareerGoal> builder)
        {
            builder.HasKey(cg => cg.Id);
            builder.Property(cg => cg.Name).IsRequired().HasMaxLength(100); 
            builder.Property(cg => cg.Description).HasMaxLength(500);

                builder.HasMany<ApplicationUser>() // This means that one CareerGoal can be associated with many ApplicationUsers
                    .WithOne(au => au.CareerGoal) // This means that each ApplicationUser has one CareerGoal
                    .HasForeignKey(au => au.CareerGoalId) // This means that CareerGoalId is a foreign key in the ApplicationUser table
                    .OnDelete(DeleteBehavior.NoAction);// This means that when a CareerGoal is deleted, the related ApplicationUsers will not be deleted and their CareerGoalId will not be set to null. This is important to prevent accidental deletion of users when a career goal is deleted.
        }
    }
}