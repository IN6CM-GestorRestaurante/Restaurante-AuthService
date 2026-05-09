using System.ComponentModel.DataAnnotations;

namespace Restaurante.AuthService.Application.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
