using Restaurante.AuthService.Domain.Entities;

namespace Restaurante.AuthService.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de envío de correos electrónicos transaccionales.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un correo de verificación de cuenta.
    /// </summary>
    Task SendEmailVerificationAsync(string email, string username, string token);

    /// <summary>
    /// Envía un correo con el enlace para restablecer la contraseña.
    /// </summary>
    Task SendPasswordResetAsync(string email, string username, string token);

    /// <summary>
    /// Envía un correo de bienvenida tras la verificación de la cuenta.
    /// </summary>
    Task SendWelcomeEmailAsync(string email, string username);
}
