namespace PetApp.Common.Models;

public interface IUserInfo
{
    string? Id { get; }
    string? Jti { get; }
    TimeSpan? Exp { get; }

    T? GetClaim<T>(string claimType) where T : IParsable<T>;
}