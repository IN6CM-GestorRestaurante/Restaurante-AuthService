using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurante.AuthService.Application.DTOs;
using Restaurante.AuthService.Application.Interfaces;

namespace Restaurante.AuthService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>
    /// Valida el token actual y devuelve la información fresca del usuario.
    /// </summary>
    [Authorize]
    [HttpGet("validate")]
    public async Task<IActionResult> Validate()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var user = await _authService.ValidateUserAsync(int.Parse(userIdClaim));
        if (user == null) return Unauthorized(new { success = false, message = "Usuario no encontrado o desactivado." });

        return Ok(new
        {
            success = true,
            user = new
            {
                id = user.Id,
                email = user.Email,
                role = user.Role,
                isActive = user.IsActive,
                emailVerified = user.EmailVerified,
                mongoId = user.MongoId,
                companyMongoId = user.CompanyMongoId
            }
        });
    }

    /// <summary>
    /// Lista usuarios de forma paginada.
    /// </summary>
    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var role = User.FindFirst("role")?.Value;
        var companyId = User.FindFirst("companyMongoId")?.Value;

        if (role != "SUPER_ADMIN" && role != "COMPANY_ADMIN")
        {
            return Forbid();
        }

        string? filterCompanyId = role == "COMPANY_ADMIN" ? companyId : null;

        var (users, totalCount) = await _authService.GetUsersAsync(page, limit, filterCompanyId);

        return Ok(new
        {
            success = true,
            users = users.Select(u => new
            {
                u.Id,
                u.Email,
                u.Username,
                u.Role,
                u.IsActive,
                u.EmailVerified,
                u.MongoId,
                u.CompanyMongoId,
                u.CreatedAt
            }),
            totalCount,
            page,
            totalPages = (int)Math.Ceiling((double)totalCount / limit)
        });
    }

    /// <summary>
    /// Actualiza el rol de un usuario.
    /// </summary>
    [Authorize]
    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto request)
    {
        var requesterRole = User.FindFirst("role")?.Value;
        var requesterCompanyId = User.FindFirst("companyMongoId")?.Value;

        if (requesterRole != "SUPER_ADMIN" && requesterRole != "COMPANY_ADMIN")
        {
            return Forbid();
        }

        try
        {
            await _authService.UpdateUserRoleAsync(id, request.Role, requesterRole!, requesterCompanyId);
            return Ok(new { success = true, message = "Rol actualizado exitosamente." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return StatusCode(201, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Autentica a un usuario y devuelve tokens.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        try
        {
            var authResponse = await _authService.LoginAsync(request);
            return Ok(new
            {
                success = true,
                message = "Login exitoso",
                accessToken = authResponse.AccessToken,
                refreshToken = authResponse.RefreshToken,
                expiresIn = authResponse.ExpiresIn,
                userDetails = authResponse.UserDetails
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Refresca el token de acceso usando el token de refresco.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto request)
    {
        try
        {
            var authResponse = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(new
            {
                success = true,
                message = "Token refrescado exitosamente",
                accessToken = authResponse.AccessToken,
                refreshToken = authResponse.RefreshToken,
                expiresIn = authResponse.ExpiresIn,
                userDetails = authResponse.UserDetails
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Verifica el correo electrónico del usuario.
    /// </summary>
    [HttpPost("verify-email")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
    {
        try
        {
            await _authService.VerifyEmailAsync(request.Token);
            return Ok(new { success = true, message = "Correo verificado exitosamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Solicita restablecer la contraseña.
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        await _authService.ForgotPasswordAsync(request.Email);
        return Ok(new { success = true, message = "Si el correo está registrado, recibirás instrucciones para restablecer tu contraseña." });
    }

    /// <summary>
    /// Restablece la contraseña usando el token.
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        try
        {
            await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            return Ok(new { success = true, message = "Contraseña restablecida exitosamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}