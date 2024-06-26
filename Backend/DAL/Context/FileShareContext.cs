using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DAL.Entities;

namespace DAL.Context
{
    public partial class FileShareContext : DbContext
    {
        public FileShareContext()
        {
        }

        public FileShareContext(DbContextOptions<FileShareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Passhash).HasColumnName("passhash");

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasDefaultValueSql("'member'");

                entity.Property(e => e.Username).HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
