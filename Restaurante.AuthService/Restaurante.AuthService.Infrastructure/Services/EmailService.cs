using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Restaurante.AuthService.Application.Interfaces;

namespace Restaurante.AuthService.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de correos usando MailKit.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var enabled = Convert.ToBoolean(smtpSettings["Enabled"]);
        if (!enabled)
        {
            _logger.LogInformation("Envío de correo deshabilitado (SmtpSettings:Enabled = false). Destinatario: {To}, Asunto: {Subject}", to, subject);
            return;
        }

        try
        {
            var message = new MimeMessage();
            var fromEmail = smtpSettings["FromEmail"] ?? "olivermerida765@gmail.com";
            var fromName = smtpSettings["FromName"] ?? "Restaurant-Auth-System";

            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            // Bypass SSL cert validation (útil para desarrollo/pruebas)
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            var host = smtpSettings["Host"];
            var port = Convert.ToInt32(smtpSettings["Port"]);
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];

            var secureSocketOptions = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await client.ConnectAsync(host, port, secureSocketOptions);

            if (!string.IsNullOrEmpty(username))
            {
                await client.AuthenticateAsync(username, password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Correo enviado exitosamente a {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo a {To}", to);
            throw;
        }
    }

    public async Task SendEmailVerificationAsync(string email, string username, string token)
    {
        var frontendUrl = _configuration["AppSettings:FrontendUrl"];
        var verifyUrl = $"{frontendUrl}/verify-email?token={token}";

        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2 style='color: #7c2d12;'>Verifica tu correo electrónico</h2>
            <p>Hola {username},</p>
            <p>Gracias por registrarte. Por favor verifica tu cuenta haciendo clic en el botón de abajo:</p>
            <a href='{verifyUrl}' style='display: inline-block; padding: 10px 20px; background-color: #7c2d12; color: #ffffff; text-decoration: none; border-radius: 5px; margin-top: 10px;'>Verificar Cuenta</a>
            <p style='margin-top: 20px; font-size: 12px; color: #666;'>Si no solicitaste esta cuenta, puedes ignorar este correo.</p>
        </div>";

        await SendEmailAsync(email, "Verificación de cuenta - Restaurante", htmlBody);
    }

    public async Task SendPasswordResetAsync(string email, string username, string token)
    {
        var frontendUrl = _configuration["AppSettings:FrontendUrl"];
        var resetUrl = $"{frontendUrl}/reset-password?token={token}";

        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2 style='color: #7c2d12;'>Restablecimiento de contraseña</h2>
            <p>Hola {username},</p>
            <p>Hemos recibido una solicitud para restablecer tu contraseña. Este enlace expirará en 1 hora.</p>
            <a href='{resetUrl}' style='display: inline-block; padding: 10px 20px; background-color: #7c2d12; color: #ffffff; text-decoration: none; border-radius: 5px; margin-top: 10px;'>Restablecer Contraseña</a>
            <p style='margin-top: 20px; font-size: 12px; color: #666;'>Si no solicitaste esto, puedes ignorar este correo.</p>
        </div>";

        await SendEmailAsync(email, "Restablecimiento de contraseña - Restaurante", htmlBody);
    }

    public async Task SendWelcomeEmailAsync(string email, string username)
    {
        var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2 style='color: #7c2d12;'>¡Bienvenido a la plataforma!</h2>
            <p>Hola {username},</p>
            <p>Tu cuenta ha sido verificada exitosamente. ¡Estamos felices de tenerte con nosotros!</p>
            <p style='margin-top: 20px; font-size: 12px; color: #666;'>El equipo del Restaurante</p>
        </div>";

        await SendEmailAsync(email, "¡Bienvenido! - Restaurante", htmlBody);
    }
}
