using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Estoque.Api.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Estoque.Api.Auth;

public sealed class TokenService : ITokenService
{
    // Mantemos as configurações necessárias para gerar o token.
    private readonly JwtSettings _settings;

    // IOptions<JwtSettings> é resolvido pela injeção de dependência.
    public TokenService(IOptions<JwtSettings> options)
    {
        // Value contém a seção Jwt já convertida para JwtSettings.
        _settings = options.Value;
    }

    public string GenerateToken(
        string username,
        string role,
        IEnumerable<string> permissions)
    {
        // Claims são informações sobre a identidade.
        // Não coloque informações secretas aqui: o payload pode ser decodificado.
        var claims = new List<Claim>
        {
            // "sub" = subject. Identifica o principal sujeito do token.
            new(JwtRegisteredClaimNames.Sub, username),

            // Nome único no exemplo didático. Em produção, normalmente use um ID estável.
            new(JwtRegisteredClaimNames.UniqueName, username),

            // ClaimTypes.Name facilita acessar context.User.Identity.Name/FindFirst.
            new(ClaimTypes.Name, username),

            // Role será usada por [Authorize(Roles = "Admin")].
            new(ClaimTypes.Role, role)
        };

        // Cada permissão vira uma claim separada.
        // Ex.: permission=estoque.read e permission=estoque.write.
        claims.AddRange(
            permissions.Select(permission => new Claim("permission", permission))
        );

        // HMAC usa a mesma chave para assinar e validar.
        // Convertendo a string configurada para bytes criamos a chave criptográfica.
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key)
        );

        // Define chave + algoritmo de assinatura.
        // A assinatura impede que alguém altere claims sem invalidar o token.
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        // Agora montamos o token completo com emissor, público, claims e expiração.
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: credentials
        );

        // WriteToken serializa o objeto para o formato compacto xxxxx.yyyyy.zzzzz.
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
