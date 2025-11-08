using ASPNetCorePermissionIdentity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPNetCorePermissionIdentity.Persistence
{
    public class PermissionIdentityDbContext<TUser, TRole, TKey>
        : IdentityDbContext<
        TUser, TRole, TKey,
        IdentityUserClaim<TKey>,
        IdentityUserRole<TKey>,
        IdentityUserLogin<TKey>,
        IdentityRoleClaim<TKey>,
        IdentityUserToken<TKey>>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>

    {
        public PermissionIdentityDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<PermissionGroup<TKey>> PermissionGroups => Set<PermissionGroup<TKey>>();
        public virtual DbSet<Permission<TKey>> Permissions => Set<Permission<TKey>>();
        public virtual DbSet<RolePermission<TKey>> RolePermissions => Set<RolePermission<TKey>>();
        public virtual DbSet<UserPermission<TKey>> UserPermissions => Set<UserPermission<TKey>>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            var schema = "identity"; 

            // PermissionGroup
            b.Entity<PermissionGroup<TKey>>(e =>
            {
                e.ToTable("PermissionGroups", schema);
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(128).IsRequired();
                e.Property(x => x.DisplayName).HasMaxLength(256);
                e.HasIndex(x => x.Name).IsUnique();
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            // Permission
            b.Entity<Permission<TKey>>(e =>
            {
                e.ToTable("Permissions", schema);
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(256).IsRequired();
                e.Property(x => x.DisplayName).HasMaxLength(256);
                e.HasIndex(x => x.Name).IsUnique();

                e.HasOne(x => x.Group)
                 .WithMany(g => g.Permissions)
                 .HasForeignKey(x => x.GroupId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasQueryFilter(x => !x.IsDeleted);
            });

            // RolePermission (Role ↔ Permission)
            b.Entity<RolePermission<TKey>>(e =>
            {
                e.ToTable("RolePermissions", schema);
                e.HasKey(x => new { x.RoleId, x.PermissionId });
                e.HasIndex(x => x.PermissionId);

                e.HasOne<TRole>()
                 .WithMany()
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<Permission<TKey>>()
                 .WithMany()
                 .HasForeignKey(x => x.PermissionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // UserPermission (User ↔ Permission)
            b.Entity<UserPermission<TKey>>(e =>
            {
                e.ToTable("UserPermissions", schema);
                e.HasKey(x => new { x.UserId, x.PermissionId });
                e.HasIndex(x => x.PermissionId);

                e.HasOne<TUser>()
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<Permission<TKey>>()
                 .WithMany()
                 .HasForeignKey(x => x.PermissionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            
            ConfigureModel(b, schema);
        }

        
        protected virtual void ConfigureModel(ModelBuilder modelBuilder, string schema) { }


    }
}
