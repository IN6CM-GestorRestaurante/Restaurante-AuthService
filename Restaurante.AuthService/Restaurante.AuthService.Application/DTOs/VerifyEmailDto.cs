using System.ComponentModel.DataAnnotations;

namespace Restaurante.AuthService.Application.DTOs;

public class VerifyEmailDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}
