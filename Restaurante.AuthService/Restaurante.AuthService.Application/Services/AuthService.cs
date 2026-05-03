using System.Globalization;
using Restaurante.AuthService.Application.DTOs;
using Restaurante.AuthService.Application.Interfaces;
using Restaurante.AuthService.Domain.Entities;

namespace Restaurante.AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly ISyncService _syncService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        ISyncService syncService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _syncService = syncService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null || !_passwordHasher.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciales incorrectas.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("El usuario está desactivado.");
        }

        var token = _jwtProvider.GenerateToken(user);
        
        return new AuthResponseDto 
        {
            Token = token,
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
            Role = registerDto.Role.ToUpper(new CultureInfo("en-US")),
            IsActive = true
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        // Llamada externa delegada a la interfaz
        await _syncService.SyncUserToMongoAsync(newUser);

        return true;
    }
}