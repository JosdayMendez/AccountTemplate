using AccountTemplate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountTemplate.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserBranch> UserBranches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Profile>()
                .HasKey(p => p.Id);

            builder.Entity<Branch>()
                .HasKey(b => b.Id);

            builder.Entity<UserBranch>()
                .HasKey(ub => ub.Id);

            builder.Entity<UserBranch>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBranches)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserBranch>()
                .HasOne(ub => ub.Branch)
                .WithMany(b => b.UserBranches) 
                .HasForeignKey(ub => ub.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
