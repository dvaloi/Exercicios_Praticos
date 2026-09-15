namespace Estoque.Api.Auth;

public interface ITokenService
{
    // Recebe os dados necessários para formar a identidade e devolve o JWT
    // serializado como string, pronto para ser enviado ao cliente.
    string GenerateToken(
        string username,
        string role,
        IEnumerable<string> permissions);
}

