using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Services;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokenService) : ControllerBase
{
    private readonly TokenService _tokenService = tokenService;

    // Usuários em memória
    private static readonly List<User> Users =
    [
        // Vendedores
        new User { Id = "1", Username = "ana.sofas", Password = "123456", Profile = "seller" },
        new User { Id = "2", Username = "beto.panelas", Password = "123456", Profile = "seller" },
        new User { Id = "3", Username = "mario.armarios", Password = "123456", Profile = "seller" },

        // Compradores
        new User { Id = "4", Username = "joao", Password = "123456", Profile = "buyer" },
        new User { Id = "5", Username = "maria", Password = "123456", Profile = "buyer" },
        new User { Id = "6", Username = "bruno", Password = "123456", Profile = "buyer" }
    ];

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto login)
    {
        var user = Users.FirstOrDefault(u => 
            u.Username == login.Username && u.Password == login.Password);
            
        if (user == null)
            return Unauthorized("Usuário ou senha inválidos");

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
    }
}