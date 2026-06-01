using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class CommunityPostReactionConfiguration : IEntityTypeConfiguration<CommunityPostReaction>
    {
        public void Configure(EntityTypeBuilder<CommunityPostReaction> builder)
        {
            builder.ToTable("CommunityPostReactions");

            builder.HasKey(x => new { x.PostId, x.UserId });

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(u => u.CommunityPostReactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
