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
        // اسم الـ schema الافتراضي
        private readonly string _schema;

        // ctor افتراضي: يستخدم "identity" كـ schema
        public PermissionIdentityDbContext(DbContextOptions options)
            : this(options, "identity") { }

        // ctor يسمح بتمرير schema من التطبيق المستهلك لو حبيت
        public PermissionIdentityDbContext(DbContextOptions options, string schema)
            : base(options)
        {
            _schema = string.IsNullOrWhiteSpace(schema) ? "identity" : schema.Trim();
        }

        public virtual DbSet<PermissionGroup<TKey>> PermissionGroups => Set<PermissionGroup<TKey>>();
        public virtual DbSet<Permission<TKey>> Permissions => Set<Permission<TKey>>();
        public virtual DbSet<RolePermission<TKey>> RolePermissions => Set<RolePermission<TKey>>();
        public virtual DbSet<UserPermission<TKey>> UserPermissions => Set<UserPermission<TKey>>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // 👈 هنا بنحدد الـ Default Schema لكل الجداول
            b.HasDefaultSchema(_schema);

            // لو عايز تسمّي جداول Identity الأساسية بنفس أسماء AspNet* تحت الـ schema الافتراضي:
            // (اختياري) 
            b.Entity<TUser>().ToTable("AspNetUsers");
            b.Entity<TRole>().ToTable("AspNetRoles");
            b.Entity<IdentityUserRole<TKey>>().ToTable("AspNetUserRoles");
            b.Entity<IdentityUserClaim<TKey>>().ToTable("AspNetUserClaims");
            b.Entity<IdentityUserLogin<TKey>>().ToTable("AspNetUserLogins");
            b.Entity<IdentityRoleClaim<TKey>>().ToTable("AspNetRoleClaims");
            b.Entity<IdentityUserToken<TKey>>().ToTable("AspNetUserTokens");

            // PermissionGroup
            b.Entity<PermissionGroup<TKey>>(e =>
            {
                e.ToTable("AspNetPermissionGroups"); // الschema الافتراضي بيتطبق تلقائياً
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(128).IsRequired();
                e.Property(x => x.DisplayName).HasMaxLength(256);
                e.HasIndex(x => x.Name).IsUnique();
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            // Permission
            b.Entity<Permission<TKey>>(e =>
            {
                e.ToTable("AspNetPermissions");
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
                e.ToTable("AspNetRolePermissions");
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
                e.ToTable("AspNetUserPermissions");
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

            // Hook للتطبيق المشتق (لو عايز تزود أي تخصيصات) — هنمرر له الschema الفعلي
            ConfigureModel(b, _schema);
        }

        protected virtual void ConfigureModel(ModelBuilder modelBuilder, string schema) { }
    }
}
