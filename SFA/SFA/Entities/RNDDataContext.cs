using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SFA.Entities
{
    public partial class RNDDataContext : DbContext
    {
        public RNDDataContext()
        {
        }

        public RNDDataContext(DbContextOptions<RNDDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblMenu> TblMenu { get; set; }
        public virtual DbSet<TblMenuGroup> TblMenuGroup { get; set; }
        public virtual DbSet<TblRole> TblRole { get; set; }
        public virtual DbSet<TblRoleMenu> TblRoleMenu { get; set; }
        public virtual DbSet<TblUser> TblUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblMenu>(entity =>
            {
                entity.ToTable("Tbl_Menu", "Auth");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.StartingPath)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Target).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TblMenuCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tbl_Menu_Tbl_User");

                entity.HasOne(d => d.LastModifiedByNavigation)
                    .WithMany(p => p.TblMenuLastModifiedByNavigation)
                    .HasForeignKey(d => d.LastModifiedBy)
                    .HasConstraintName("FK_Tbl_Menu_Tbl_User1");

                entity.HasOne(d => d.MenuGroup)
                    .WithMany(p => p.TblMenu)
                    .HasForeignKey(d => d.MenuGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tbl_Menu_Tbl_MenuGroup");
            });

            modelBuilder.Entity<TblMenuGroup>(entity =>
            {
                entity.ToTable("Tbl_MenuGroup", "Auth");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Category).HasMaxLength(100);

                entity.Property(e => e.Icon).HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Target).HasMaxLength(100);
            });

            modelBuilder.Entity<TblRole>(entity =>
            {
                entity.ToTable("Tbl_Role", "Auth");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DataAccessCode)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblRoleMenu>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.MenuId });

                entity.ToTable("Tbl_RoleMenu", "Auth");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.TblRoleMenu)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tbl_RoleMenu_Tbl_Menu");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TblRoleMenu)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tbl_RoleMenu_Role");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.ToTable("Tbl_User", "Auth");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TblUser)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tbl_User_Tbl_Role");
            });
        }
    }
}
