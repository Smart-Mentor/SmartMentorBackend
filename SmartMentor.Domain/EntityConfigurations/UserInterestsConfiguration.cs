using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class UserInterestsConfiguration: IEntityTypeConfiguration<UserInterests>
    {
        public void Configure(EntityTypeBuilder<UserInterests> builder)
        {
            builder.HasKey(ui => new{ ui.UserId, ui.InterestId }); // Composite key of UserId and InterestId
            builder.HasOne(ui => ui.User)
                     .WithMany(u => u.UserInterests)
                   .HasForeignKey(ui => ui.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ui => ui.Interest)
                     .WithMany(i => i.UserInterests)
                   .HasForeignKey(ui => ui.InterestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}