namespace Restaurante.AuthService.Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDetailsDto UserDetails { get; set; } = new UserDetailsDto();
    }

    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? MongoId { get; set; }
    }
}