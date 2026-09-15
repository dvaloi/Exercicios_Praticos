using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Estoque.Api.Auth;
using Estoque.Api.Configuration;
using Estoque.Api.Health;
using Estoque.Api.Middleware;
using Estoque.Api.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
    };
});

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName)
);

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração Jwt ausente.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException(
        "Jwt:Key não configurada. Use User Secrets ou variável de ambiente."
    );
}

if (Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key deve possuir pelo menos 32 bytes para este exemplo."
    );
}

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)
            ),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "CanReadStock",
        policy => policy.RequireClaim("permission", "estoque.read")
    );

    options.AddPolicy(
        "CanWriteStock",
        policy => policy.RequireClaim("permission", "estoque.write")
    );

    options.AddPolicy(
        "CanDeleteStock",
        policy => policy
            .RequireRole("Admin")
            .RequireClaim("permission", "estoque.delete")
    );
});
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext =>
        {
            var partitionKey =
                httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }
            );
        }
    );

    options.AddFixedWindowLimiter("LoginPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });
});

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;

        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<
    IConfigureOptions<SwaggerGenOptions>,
    ConfigureSwaggerOptions>();

builder.Services
    .AddHealthChecks()
    .AddCheck<SelfHealthCheck>("self");

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Headers defensivos.
app.UseMiddleware<SecurityHeadersMiddleware>();

// Redireciona HTTP -> HTTPS.
app.UseHttpsRedirection();

// Swagger somente em Development.
if (app.Environment.IsDevelopment())
{
    var versionProvider = app.Services
        .GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        foreach (var description in versionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant()
            );
        }
    });
}

app.UseCors("Frontend");
// Rate limiting.
app.UseRateLimiter();

// Autenticação lê/valida o Bearer token e preenche HttpContext.User.
app.UseAuthentication();

// Autorização usa HttpContext.User para verificar [Authorize], Roles e Policies.
// Por isso ela vem DEPOIS de UseAuthentication().
app.UseAuthorization();

// Auditoria depois da autenticação, para conseguir registrar o usuário.
app.UseMiddleware<AuditMiddleware>();

// Métricas automáticas de HTTP.
app.UseHttpMetrics();

app.MapControllers();

// Health endpoint.
app.MapHealthChecks("/health").AllowAnonymous();

// Endpoint de métricas Prometheus.
app.MapMetrics("/metrics").AllowAnonymous();

app.Run();
