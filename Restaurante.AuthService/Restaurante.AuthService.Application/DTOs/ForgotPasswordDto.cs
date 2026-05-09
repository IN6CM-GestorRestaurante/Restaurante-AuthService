using System.ComponentModel.DataAnnotations;

namespace Restaurante.AuthService.Application.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
