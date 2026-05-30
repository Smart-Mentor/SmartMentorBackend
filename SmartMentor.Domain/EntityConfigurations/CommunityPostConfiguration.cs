using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class CommunityPostConfiguration : IEntityTypeConfiguration<CommunityPost>
    {
        public void Configure(EntityTypeBuilder<CommunityPost> builder)
        {
            builder.ToTable("CommunityPosts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.HasOne(x => x.Author)
                .WithMany(u => u.CommunityPosts)
                .HasForeignKey(x => x.AuthorUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.PrimaryCareerGoal)
                .WithMany(cg => cg.PrimaryCommunityPosts)
                .HasForeignKey(x => x.PrimaryCareerGoalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
