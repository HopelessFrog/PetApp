using Hangfire;
using Microsoft.EntityFrameworkCore;
using PetApp.AuthService.Data;

namespace PetApp.AuthService.Jobs;

public class TokenCleanupJob
{
    private readonly AuthDbContext _dbContext;

    public TokenCleanupJob(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task CleanupExpiredAndRevokedTokensAsync()
    {
        var currentTime = DateTime.UtcNow;

        var deletedCount = await _dbContext.RefreshTokens
            .Where(t => t.IsRevoked || t.ExpiresAt < currentTime)
            .ExecuteDeleteAsync();
    }
}