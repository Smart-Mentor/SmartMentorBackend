using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Domain.EntityConfigurations
{
    public class CommunityPostCareerGoalTagConfiguration : IEntityTypeConfiguration<CommunityPostCareerGoalTag>
    {
        public void Configure(EntityTypeBuilder<CommunityPostCareerGoalTag> builder)
        {
            builder.ToTable("CommunityPostCareerGoalTags");

            builder.HasKey(x => new { x.PostId, x.CareerGoalId });

            builder.HasOne(x => x.Post)
                .WithMany(p => p.CareerGoalTags)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CareerGoal)
                .WithMany(cg => cg.TaggedCommunityPosts)
                .HasForeignKey(x => x.CareerGoalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
