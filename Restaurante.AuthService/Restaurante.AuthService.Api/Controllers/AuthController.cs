using Microsoft.AspNetCore.Mvc;
using Restaurante.AuthService.Application.DTOs;
using Restaurante.AuthService.Application.Interfaces;

namespace Restaurante.AuthService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    /// <param name="request">DTO con los datos del usuario a registrar.</param>
    /// <returns>Devuelve un mensaje de éxito o error.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return StatusCode(201, new { message = "Usuario registrado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Autentica a un usuario y devuelve un token JWT.
    /// </summary>
    /// <param name="request">DTO con los datos del usuario a autenticar.</param>
    /// <returns>Devuelve un token JWT.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        try
        {
            var authResponse = await _authService.LoginAsync(request);
            return Ok(new
            {
                message = "Login exitoso",
                token = authResponse.Token,
                userDetails = authResponse.UserDetails
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}