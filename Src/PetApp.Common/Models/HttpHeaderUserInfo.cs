using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.JsonWebTokens;
using PetApp.Common.Constants;

namespace PetApp.Common.Models;

public class HttpHeaderUserInfo : IUserInfo
{
    private readonly Dictionary<string, object?> _claims;

    public string? Id => GetClaim<string>(ClaimTypes.NameIdentifier);
    public string? Jti => GetClaim<string>(JwtRegisteredClaimNames.Jti);

    public TimeSpan? Exp =>
        DateTimeOffset.FromUnixTimeSeconds(GetClaim<long>(JwtRegisteredClaimNames.Exp)).UtcDateTime - DateTime.UtcNow;

    public HttpHeaderUserInfo(IHttpContextAccessor accessor)
    {
        var headers = accessor.HttpContext?.Request.Headers;
        var claimsJson = headers?[ForwardedHeadersHttp.Claims].FirstOrDefault();

        _claims = string.IsNullOrEmpty(claimsJson)
            ? new Dictionary<string, object?>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(claimsJson);
    }

    public T? GetClaim<T>(string claimType) where T : IParsable<T>
    {
        if (!_claims.TryGetValue(claimType, out var value) || value is null)
            return default(T);

        return T.TryParse(value.ToString(), null, out var result) ? result : default(T);
    }
}