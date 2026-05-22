namespace Restaurante.AuthService.Application.DTOs;
/// <summary>
/// Respuesta del registro para consumo del orquestador (Node.js).
/// Devuelve el ID generado en Postgres para que Node lo vincule como authId.
/// </summary>
public class RegisterResponseDto
{
    public bool Success { get; set; }
    public Guid AuthUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
