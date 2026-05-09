using System.Globalization;
using Microsoft.Extensions.Configuration;
using Restaurante.AuthService.Application.DTOs;
using Restaurante.AuthService.Application.Helpers;
using Restaurante.AuthService.Application.Interfaces;
using Restaurante.AuthService.Domain.Entities;

namespace Restaurante.AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly ISyncService _syncService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        ISyncService syncService,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _syncService = syncService;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        User? user;
        if (loginDto.EmailOrUsername.Contains('@'))
        {
            user = await _userRepository.GetByEmailAsync(loginDto.EmailOrUsername);
        }
        else
        {
            user = await _userRepository.GetByUsernameAsync(loginDto.EmailOrUsername);
        }

        if (user == null || !_passwordHasher.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciales incorrectas.");
        }

        if (!user.EmailVerified)
        {
            throw new UnauthorizedAccessException("Debes verificar tu correo antes de iniciar sesión.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("El usuario está desactivado.");
        }

        var token = _jwtProvider.GenerateToken(user);
        
        user.RefreshToken = TokenGenerator.GenerateSecureToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        int expiresInMinutes = int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int exp) ? exp : 480;

        return new AuthResponseDto 
        {
            AccessToken = token,
            RefreshToken = user.RefreshToken,
            ExpiresIn = expiresInMinutes * 60,
            UserDetails = new UserDetailsDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                MongoId = user.MongoId
            }
        };
    }

    public async Task<bool> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("El correo ya está registrado.");
        }

        var newUser = new User
        {
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.Hash(registerDto.Password),
            Role = "COMPANY_ADMIN",
            IsActive = false,
            EmailVerified = false,
            EmailVerificationToken = TokenGenerator.GenerateSecureToken(),
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        Task.Run(() => _emailService.SendEmailVerificationAsync(newUser.Email, newUser.Username ?? newUser.Email, newUser.EmailVerificationToken));

        await _syncService.SyncUserToMongoAsync(newUser);

        return true;
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Token de refresco inválido o expirado.");
        }

        var token = _jwtProvider.GenerateToken(user);
        user.RefreshToken = TokenGenerator.GenerateSecureToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        int expiresInMinutes = int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int exp) ? exp : 480;

        return new AuthResponseDto 
        {
            AccessToken = token,
            RefreshToken = user.RefreshToken,
            ExpiresIn = expiresInMinutes * 60,
            UserDetails = new UserDetailsDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                MongoId = user.MongoId
            }
        };
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _userRepository.GetByEmailVerificationTokenAsync(token);

        if (user == null || user.EmailVerificationTokenExpiry < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Token inválido o expirado.");
        }

        user.EmailVerified = true;
        user.IsActive = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        Task.Run(() => _emailService.SendWelcomeEmailAsync(user.Email, user.Username ?? user.Email));

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            return true;
        }

        user.PasswordResetToken = TokenGenerator.GenerateSecureToken();
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        Task.Run(() => _emailService.SendPasswordResetAsync(user.Email, user.Username ?? user.Email, user.PasswordResetToken));

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(token);

        if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Token inválido o expirado.");
        }

        user.PasswordHash = _passwordHasher.Hash(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<User?> ValidateUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive) return null;
        return user;
    }

    public async Task<(List<User> Users, int TotalCount)> GetUsersAsync(int page, int limit, string? companyMongoId = null)
    {
        return await _userRepository.GetAllAsync(page, limit, companyMongoId);
    }

    public async Task<bool> UpdateUserRoleAsync(int userId, string newRole, string requesterRole, string? requesterCompanyId)
    {
        var allowedRoles = new[] { "SUPER_ADMIN", "COMPANY_ADMIN", "BRANCH_MANAGER", "WAITER", "CHEF", "CASHIER", "RECEPTIONIST", "CLIENT" };
        newRole = newRole.ToUpper(CultureInfo.InvariantCulture);

        if (!allowedRoles.Contains(newRole))
        {
            throw new InvalidOperationException($"El rol '{newRole}' no es válido.");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new InvalidOperationException("Usuario no encontrado.");

        // Solo SUPER_ADMIN puede asignar COMPANY_ADMIN
        if (newRole == "COMPANY_ADMIN" && requesterRole != "SUPER_ADMIN")
        {
            throw new UnauthorizedAccessException("Solo un SUPER_ADMIN puede asignar el rol COMPANY_ADMIN.");
        }

        // Si es COMPANY_ADMIN, solo puede editar usuarios de su misma compañía
        if (requesterRole == "COMPANY_ADMIN" && user.CompanyMongoId != requesterCompanyId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para modificar usuarios de otra compañía.");
        }

        user.Role = newRole;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }
}