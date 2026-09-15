using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Estoque.Api.Health;

public sealed class SelfHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            HealthCheckResult.Healthy("API está respondendo.")
        );
    }
}
