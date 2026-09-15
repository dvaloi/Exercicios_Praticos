namespace Estoque.Api.Contracts;

public sealed record LoginResponse(
    // Token que será usado nas próximas requisições.
    string AccessToken,

    // Padrão HTTP utilizado no header Authorization: Bearer <token>.
    string TokenType,

    // Informa ao cliente a duração configurada neste exemplo.
    int ExpiresInMinutes
);
