using System;
using Microsoft.EntityFrameworkCore;

namespace KBT.WebAPI.Training.Example.Entities.JWT
{
    public class JwtDbContext : DbContext
    {
        public JwtDbContext()
        {
        }

        public JwtDbContext(DbContextOptions<JwtDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<JwtToken> JwtToken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("Data Source=blogging.db");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JwtToken>(entity =>
            {
                entity.Property(e => e.Key).ValueGeneratedOnAdd();
                entity.HasKey(e => e.Key).HasName("PK__JWTToken__C41E02882E3E26B6");
                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Expiration).HasColumnType("datetime");

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
        }
    }
}

