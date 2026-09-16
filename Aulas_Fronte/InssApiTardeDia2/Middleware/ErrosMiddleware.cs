namespace InssApi.Middleware;

/// <summary>Portão de erros — o Swagger vê mensagem; o detalhe vai ao log com correlação.</summary>
public class ErrosMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrosMiddleware> _logger;

    public ErrosMiddleware(RequestDelegate next, ILogger<ErrosMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha não tratada. TraceId={TraceId}", context.TraceIdentifier);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                erro = "Erro interno. Tente novamente.",
                correlacao = context.TraceIdentifier
            });
        }
    }
}
