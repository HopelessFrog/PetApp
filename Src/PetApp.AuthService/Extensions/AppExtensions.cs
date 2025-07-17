using Hangfire;
using PetApp.AuthService.Jobs;

namespace PetApp.AuthService.Extensions;

public static class AppExtensions
{
    public static void ConfigureTokenCleanupJob(this IApplicationBuilder app)
    {
        RecurringJob.AddOrUpdate<TokenCleanupJob>(
            "token-cleanup",
            job => job.CleanupExpiredAndRevokedTokensAsync(),
            "* * * * *");
    }
}