using Restaurante.AuthService.Application.DTOs;

namespace Restaurante.AuthService.Application.Interfaces;

public interface IAuthService
{
    // Devuelve un DTO con el token JWT y los datos del usuario al hacer login
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    
    // Devuelve true/false o un DTO de respuesta al registrar
    Task<bool> RegisterAsync(RegisterDto registerDto);
}