using System.ComponentModel.DataAnnotations;

namespace Restaurante.AuthService.Application.DTOs;

public class ResendVerificationDto
{
    [Required]
    public string Email { get; set; } = string.Empty;
}
