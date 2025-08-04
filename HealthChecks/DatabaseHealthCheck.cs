using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

public class DatabaseHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Здесь реализуйте проверку подключения к БД
        // Это пример - замените на реальную проверку

        try
        {
            // Проверка подключения к БД
            // Например, выполнить простой запрос "SELECT 1"

            return Task.FromResult(
                HealthCheckResult.Healthy("Database connection is OK"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy("Database connection failed", ex));
        }
    }
}