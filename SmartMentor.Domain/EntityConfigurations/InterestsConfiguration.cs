using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class InterestsConfiguration : IEntityTypeConfiguration<Interests>
    {
        public void Configure(EntityTypeBuilder<Interests> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
            
        }
    }
}