using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UsersManagementApi.Models;

namespace UsersManagementApi.Data;

public partial class Context : DbContext
{
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCredential> UserCredentials { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorLogId).HasName("PK__ErrorLog__D65247C23CAA00AA");

            entity.ToTable("ErrorLog");

            entity.Property(e => e.ErrorLogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ErrorMessage).HasMaxLength(4000);
            entity.Property(e => e.ProcedureName).HasMaxLength(128);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Logs__5E548648FC3D26F4");

            entity.Property(e => e.Category).HasMaxLength(500);
            entity.Property(e => e.LogLevel).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Password__658FEEEAA6D69F28");

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.HasIndex(e => e.ResetToken, "UQ__Password__0395685B6E5F5310").IsUnique();

            entity.Property(e => e.TokenId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.ResetToken).HasMaxLength(500);
            entity.Property(e => e.UsedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PasswordResetTokens_Users");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB2F069394AD");

            entity.HasIndex(e => e.PermissionName, "UQ__Permissi__0FFDA35733EFE27B").IsUnique();

            entity.Property(e => e.PermissionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Module).HasMaxLength(50);
            entity.Property(e => e.PermissionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AEDFCEB64");

            entity.HasIndex(e => e.RoleName, "UQ__RoleName").IsUnique();

            entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK__RolePerm__120F46BA210DD0E4");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "UQ__RolePerm__6400A1A9F18A76BC").IsUnique();

            entity.Property(e => e.RolePermissionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.GrantedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Permissions");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RolePermissions_Roles");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Subscrip__755C22B78B752D48");

            entity.HasIndex(e => e.PlanName, "UQ__PlanName").IsUnique();

            entity.Property(e => e.PlanId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AnnualPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MonthlyPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PlanName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CE2EB51C8");

            entity.HasIndex(e => e.Email, "IX_Email");

            entity.HasIndex(e => e.Email, "UQ_Email").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.EmailVerified).HasDefaultValue(false);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.HasKey(e => e.CredentialId).HasName("PK__CredentialId");

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.HasIndex(e => e.UserId, "UQ_UserCredentials_UserId").IsUnique();

            entity.Property(e => e.CredentialId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AccountLockedUntil).HasColumnType("datetime");
            entity.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);
            entity.Property(e => e.LastPasswordChangeDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithOne(p => p.UserCredential)
                .HasForeignKey<UserCredential>(d => d.UserId)
                .HasConstraintName("FK_UserCredentials_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRoleId");

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ__UserRole__AF2760ACDB201E53").IsUnique();

            entity.Property(e => e.UserRoleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__SessionId");

            entity.ToTable(tb => tb.HasTrigger("TRG_UserSessions_AfterInsert"));

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.HasIndex(e => e.RefreshTokenHash, "UQ__RefreshTokenHash").IsUnique();

            entity.Property(e => e.SessionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.RefreshTokenHash).HasMaxLength(500);
            entity.Property(e => e.RevokedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserId");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__SubscriptionId");

            entity.HasIndex(e => e.UserId, "UQ_UserSubscriptions_ActiveUser")
                .IsUnique()
                .HasFilter("([SubscriptionStatus]<>'Cancelled')");

            entity.Property(e => e.SubscriptionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.AutoRenew).HasDefaultValue(true);
            entity.Property(e => e.BillingCycle)
                .HasMaxLength(20)
                .HasDefaultValue("Monthly");
            entity.Property(e => e.CancelledDate).HasPrecision(3);
            entity.Property(e => e.CreatedDate)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EndDate).HasPrecision(3);
            entity.Property(e => e.NextBillingDate).HasPrecision(3);
            entity.Property(e => e.StartDate)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.SubscriptionStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Trial")
                .HasComment("Trial, Active, Expired, Cancelled, PastDue");
            entity.Property(e => e.TrialEndDate).HasPrecision(3);
            entity.Property(e => e.UpdatedDate)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSubscriptions_Plans");

            entity.HasOne(d => d.User).WithOne(p => p.UserSubscription)
                .HasForeignKey<UserSubscription>(d => d.UserId)
                .HasConstraintName("FK_UserSubscriptions_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
