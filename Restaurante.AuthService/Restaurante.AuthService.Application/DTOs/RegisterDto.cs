using System.ComponentModel.DataAnnotations;
namespace Restaurante.AuthService.Application.DTOs;
/// <summary>
/// DTO para crear credenciales en PostgreSQL.
/// Node.js (orquestador) envía los campos de autenticación.
/// Los datos de perfil (name, surname, phone) viven en MongoDB.
/// </summary>
public class RegisterDto
{
    /// <summary>Correo electrónico (puente entre Postgres y Mongo)</summary>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;
    /// <summary>Contraseña (se hashea con BCrypt antes de guardar)</summary>
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;
    /// <summary>Nombre de usuario para login rápido (opcional)</summary>
    [MaxLength(50, ErrorMessage = "El username no puede exceder 50 caracteres")]
    public string? Username { get; set; }
    /// <summary>ID del User en MongoDB (enviado por Node para vincular Postgres↔Mongo)</summary>
    public string? MongoId { get; set; }
    /// <summary>ID del Company en MongoDB (enviado por Node tras crear la Company)</summary>
    public string? CompanyMongoId { get; set; }
    /// <summary>Rol del usuario (enviado por Node al crear empleados, default: COMPANY_ADMIN)</summary>
    public string? Role { get; set; }
}