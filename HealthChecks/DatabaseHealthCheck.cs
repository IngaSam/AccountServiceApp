using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;


namespace AccountService.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public DatabaseHealthCheck(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (DbException ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}