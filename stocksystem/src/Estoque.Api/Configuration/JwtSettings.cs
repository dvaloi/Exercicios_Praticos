namespace Estoque.Api.Configuration;
public sealed class JwtSettings
{
    // Nome exato da seção que será lida do appsettings/User Secrets.
    // Ter a string em um único lugar evita repetir "Jwt" em vários arquivos.
    public const string SectionName = "Jwt";

    // Identifica quem emitiu o token. Na validação, rejeitamos tokens
    // emitidos por outro emissor.
    public string Issuer { get; init; } = string.Empty;

    // Identifica para qual sistema/cliente o token foi criado.
    public string Audience { get; init; } = string.Empty;

    // Segredo usado para assinatura HMAC neste exemplo.
    // IMPORTANTE: o valor real vem de User Secrets/variável de ambiente,
    // não do Git.
    public string Key { get; init; } = string.Empty;

    // Tempo de vida curto reduz a janela de uso de um token roubado.
    public int ExpirationMinutes { get; init; } = 60;
}
