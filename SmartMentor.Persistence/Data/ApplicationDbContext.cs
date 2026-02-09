using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartMentor.Domain.Entiies;
using SmartMentor.Domain.EntityConfigurations;
using SmartMentor.Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(SkillConfiguration).Assembly);
        }


        // Add the Tables You want to add to the database
            public DbSet<CareerGoal> CareerGoals { get; set; }
            public DbSet<Interests> Interests { get; set; }
            public DbSet<Skill> Skills { get; set; }
            public DbSet<UserInterests> UserInterests { get; set; }
            public DbSet<UserSkills> UserSkills { get; set; }
            

    }
}
