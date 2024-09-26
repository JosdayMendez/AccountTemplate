using AccountTemplate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountTemplate.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<ProfileBranch> ProfileBranches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Profile>()
                .ToTable("Profiles")
                .HasKey(p => p.Id);

            builder.Entity<Branch>()
               .ToTable("Branches")
               .HasKey(b => b.Id);

            builder.Entity<ProfileBranch>()
                .ToTable("ProfileBranches")
                .HasKey(pb => pb.Id);

            builder.Entity<ProfileBranch>()
                .HasOne(pb => pb.Profile)
                .WithMany(p => p.ProfileBranches)
                .HasForeignKey(pb => pb.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProfileBranch>()
                .HasOne(pb => pb.Branch)
                .WithMany(b => b.ProfileBranches)
                .HasForeignKey(pb => pb.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}