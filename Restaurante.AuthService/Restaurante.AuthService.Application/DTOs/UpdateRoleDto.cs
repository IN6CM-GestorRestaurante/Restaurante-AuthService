using System.ComponentModel.DataAnnotations;

namespace Restaurante.AuthService.Application.DTOs;

public class UpdateRoleDto
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
