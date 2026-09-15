using Estoque.Api.Auth;
using Estoque.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Api.Controllers;

// [ApiController] ativa comportamentos próprios de API, como binding/validação HTTP.
[ApiController]
// Endpoint base: /api/auth
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AuthController(
        IConfiguration configuration,
        ITokenService tokenService)
    {
        _configuration = configuration;
        _tokenService = tokenService;
    }

    // POST /api/auth/login
    [HttpPost("login")]
    // Login precisa ser acessível antes de existir uma identidade autenticada.
    [AllowAnonymous]
    public IActionResult Login(LoginRequest request)
    {
        // Para a aula, as credenciais vêm da configuração/User Secrets.
        // Em produção, consulte um Identity Provider ou armazenamento seguro.
        var expectedUsername = _configuration["DemoUser:Username"];
        var expectedPassword = _configuration["DemoUser:Password"];
        var role = _configuration["DemoUser:Role"] ?? "User";

        // Se usuário OU senha estiver incorreto, devolvemos 401.
        // Evitamos dizer qual dos dois está errado para não ajudar enumeração de usuários.
        if (request.Username != expectedUsername ||
            request.Password != expectedPassword)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Credenciais inválidas",
                Detail = "Usuário ou senha inválidos.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        // No exemplo, traduzimos role em permissões. Em um sistema real,
        // isso poderia vir do banco/IdP e ser modelado com muito mais cuidado.
        var permissions = role == "Admin"
            ? new[] { "estoque.read", "estoque.write", "estoque.delete" }
            : new[] { "estoque.read" };

        // Só geramos o token depois que as credenciais foram validadas.
        var token = _tokenService.GenerateToken(
            request.Username,
            role,
            permissions
        );

        return Ok(new LoginResponse(
            token,
            "Bearer",
            60
        ));
    }
}

