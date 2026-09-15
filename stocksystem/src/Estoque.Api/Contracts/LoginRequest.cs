namespace Estoque.Api.Contracts;

public sealed record LoginRequest(
    string Username,
    string Password);
