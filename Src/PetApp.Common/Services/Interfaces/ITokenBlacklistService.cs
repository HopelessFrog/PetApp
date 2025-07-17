namespace PetApp.Common.Services.Interfaces;

public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string jti, TimeSpan expiration);
    Task<bool> IsTokenBlacklistedAsync(string jti);
}