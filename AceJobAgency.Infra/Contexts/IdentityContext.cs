using AceJobAgency.Infra.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AceJobAgency.Infra.Contexts
{
    public class IdentityContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        public override DbSet<Role> Roles { get; set; }
        public override DbSet<User> Users { get; set; }
        public override DbSet<UserLogin> UserLogins { get; set; }
        public override DbSet<UserClaim> UserClaims { get; set; }
        public override DbSet<RoleClaim> RoleClaims { get; set; }
        public override DbSet<UserRole> UserRoles { get; set; }
        public override DbSet<UserToken> UserTokens { get; set; }
        public DbSet<UserAudit> UserAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserToken>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasKey(t => new { t.RoleId, t.UserId });
            });

            modelBuilder.Entity<Role>().HasData(
                 new Role() { Name = "Admin", Id = 1, NormalizedName = "ADMIN" },
                 new Role() { Name = "Applicant", Id = 2, NormalizedName = "APPLICANT" }
                );

            modelBuilder.HasDefaultSchema("Identity");
        }
    }
}