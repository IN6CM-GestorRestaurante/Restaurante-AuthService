using Microsoft.EntityFrameworkCore;
using Restaurante.AuthService.Domain.Entities;

namespace Restaurante.AuthService.Infrastructure.Data;
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique()
                .HasFilter("username IS NOT NULL");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.EmailVerificationToken)
                .IsUnique()
                .HasFilter("email_verification_token IS NOT NULL");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PasswordResetToken)
                .IsUnique()
                .HasFilter("password_reset_token IS NOT NULL");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.RefreshToken)
                .IsUnique()
                .HasFilter("refresh_token IS NOT NULL");
        }
    }
