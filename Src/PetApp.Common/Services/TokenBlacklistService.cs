using PetApp.Common.Services.Interfaces;
using StackExchange.Redis;

namespace PetApp.Common.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IDatabase _database;

    public TokenBlacklistService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task BlacklistTokenAsync(string jti, TimeSpan expiration)
    {
        await _database.StringSetAsync(jti.ToString(), "blacklisted", expiration);
    }

    public async Task<bool> IsTokenBlacklistedAsync(string jti)
    {
        return await _database.KeyExistsAsync(jti.ToString());
    }
}