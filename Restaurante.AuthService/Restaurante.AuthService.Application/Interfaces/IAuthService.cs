using Restaurante.AuthService.Application.DTOs;
using Restaurante.AuthService.Domain.Entities;

namespace Restaurante.AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string token, string newPassword);
    
    // Métodos administrativos e introspección
    Task<User?> ValidateUserAsync(int userId);
    Task<(List<User> Users, int TotalCount)> GetUsersAsync(int page, int limit, string? companyMongoId = null);
    Task<bool> UpdateUserRoleAsync(int userId, string newRole, string requesterRole, string? requesterCompanyId);
}