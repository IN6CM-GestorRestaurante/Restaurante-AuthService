namespace Restaurante.AuthService.Application.DTOs;

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public UserDetailsDto UserDetails { get; set; } = new UserDetailsDto();
    }

    public class UserDetailsDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? MongoId { get; set; }
    }
