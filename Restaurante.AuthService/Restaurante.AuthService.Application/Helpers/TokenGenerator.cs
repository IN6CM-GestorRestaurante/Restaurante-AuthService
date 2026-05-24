using System.Security.Cryptography;

namespace Restaurante.AuthService.Application.Helpers;

public static class TokenGenerator
{
    public static string GenerateSecureToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
           .Replace("+", "-").Replace("/", "_").TrimEnd('=');

    public static string GenerateNumericOtp()
    {
        var bytes = RandomNumberGenerator.GetBytes(4);
        var value = BitConverter.ToUInt32(bytes, 0) % 1000000;
        return value.ToString("D6");
    }
}
