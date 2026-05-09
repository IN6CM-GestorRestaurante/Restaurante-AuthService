using System.ComponentModel.DataAnnotations;

namespace Restaurante.AuthService.Application.DTOs;

    public class LoginDto
    {
        [Required(ErrorMessage = "El email o nombre de usuario es obligatorio")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
