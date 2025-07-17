using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PetApp.Common.Migrations;

public class Migrator<T>(IServiceProvider provider, ILogger<Migrator<T>> logger) : BackgroundService where T : DbContext
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();
            await context.Database.ApplyMigrations(Assembly.GetAssembly(this.GetType())!.GetName().Name!,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations");
            Environment.FailFast("Failed to apply migrations", ex);
        }
    }
}