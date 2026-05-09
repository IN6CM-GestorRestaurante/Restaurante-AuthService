using System.Security.Cryptography;

namespace Restaurante.AuthService.Application.Helpers;

public static class TokenGenerator
{
    public static string GenerateSecureToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
           .Replace("+", "-").Replace("/", "_").TrimEnd('=');
}
