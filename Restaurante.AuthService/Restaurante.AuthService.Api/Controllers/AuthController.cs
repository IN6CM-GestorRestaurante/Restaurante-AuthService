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
    /// Registra un nuevo usuario.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return StatusCode(201, new { success = true, message = "Usuario registrado. Revisa tu correo para verificar tu cuenta." });
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