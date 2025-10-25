using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [MinLength(3, ErrorMessage = "O nome de usuário deve ter no mínimo 3 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }
    }
}