using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class EmailVerificationCodesConfiguration : IEntityTypeConfiguration<EmailVerificationCodes>
    {
        public void Configure(EntityTypeBuilder<EmailVerificationCodes> builder)
        {
            builder.HasKey(e => e.Guid);
            builder.Property(e => e.Code).IsRequired();
            builder.Property(e => e.ExpirationDate).IsRequired();
            builder.Property(e => e.IsUsed).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();

            // Configure the relationship with ApplicationUser
            builder.HasOne(e => e.User)
                   .WithMany() // Assuming ApplicationUser does not have a collection of EmailVerificationCodes
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Optional: specify delete behavior
        }
    }
}
