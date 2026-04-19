using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class PasswordResetCodesConfiguration : IEntityTypeConfiguration<PasswordResetCodes>
    {
        public void Configure(EntityTypeBuilder<PasswordResetCodes> builder)
        {
            builder.HasKey(prc => prc.Id);
            builder.Property(prc => prc.Code).IsRequired();
            builder.Property(prc => prc.ResetToken).IsRequired();
            builder.Property(prc => prc.CreatedAt).IsRequired();
            builder.Property(prc => prc.Expiration).IsRequired();
            builder.Property(prc => prc.IsUsed).IsRequired();

            builder.HasOne(prc => prc.User)
                .WithMany()
                .HasForeignKey(prc => prc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}