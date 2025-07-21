using Hangfire;
using Microsoft.Extensions.Options;
using PetApp.AuthService.Jobs;
using PetApp.AuthService.Models.Options;

namespace PetApp.AuthService.Extensions;

public static class AppExtensions
{
    public static void ConfigureTokenCleanupJob(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<TokenCleanupOptions>>()
            .Value;
        
        var recurringJobManager = app.ApplicationServices
            .GetRequiredService<IRecurringJobManager>();
        
        recurringJobManager.AddOrUpdate<TokenCleanupJob>(
            options.JobId,
            job => job.CleanupExpiredAndRevokedTokensAsync(),
            options.CronExpression);
    }
}