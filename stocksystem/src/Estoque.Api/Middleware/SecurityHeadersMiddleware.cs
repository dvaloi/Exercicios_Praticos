namespace Estoque.Api.Middleware;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Este middleware roda para cada requisição que passa pelo pipeline.
        // Em vez de repetir headers em todos os controllers, centralizamos aqui.
        context.Response.Headers.TryAdd(
            "X-Content-Type-Options",
            "nosniff"
        );

        context.Response.Headers.TryAdd(
            "X-Frame-Options",
            "DENY"
        );

        context.Response.Headers.TryAdd(
            "Referrer-Policy",
            "no-referrer"
        );

        context.Response.Headers.TryAdd(
            "Permissions-Policy",
            "camera=(), microphone=(), geolocation=()"
        );

        // Chama o próximo middleware/endpoint. Sem isso, a requisição pararia aqui.
        await _next(context);
    }
}
