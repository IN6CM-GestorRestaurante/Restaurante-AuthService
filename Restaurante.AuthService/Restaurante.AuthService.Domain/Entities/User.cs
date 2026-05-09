using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurante.AuthService.Domain.Entities;

    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("role")]
        public string Role { get; set; } = "WAITER"; // ADMIN, WAITER, CHEF, CASHIER

        [Column("is_active")]
        public bool IsActive { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Para mantener la relación con el perfil guardado en MongoDB
        [Column("mongo_id")]
        public string? MongoId { get; set; } 

        // === Username (para login rápido sin email completo) ===
        [Column("username")]
        [MaxLength(50)]
        public string? Username { get; set; }

        // === Email Verification ===
        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [Column("email_verification_token")]
        public string? EmailVerificationToken { get; set; }

        [Column("email_verification_token_expiry")]
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        // === Password Reset ===
        [Column("password_reset_token")]
        public string? PasswordResetToken { get; set; }

        [Column("password_reset_token_expiry")]
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // === Refresh Token ===
        [Column("refresh_token")]
        public string? RefreshToken { get; set; }

        [Column("refresh_token_expiry")]
        public DateTime? RefreshTokenExpiry { get; set; }

        // === Tenant Link ===
        [Column("company_mongo_id")]
        public string? CompanyMongoId { get; set; }
    }