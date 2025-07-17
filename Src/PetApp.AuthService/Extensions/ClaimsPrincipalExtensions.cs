using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PetApp.AuthService.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static TimeSpan? UntilExpiration(this ClaimsPrincipal claims)
    {
        var expClaim = claims.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (string.IsNullOrEmpty(expClaim) ||
            !long.TryParse(expClaim, out var exp))
        {
            return null;
        }

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
        return expirationTime - DateTimeOffset.UtcNow;
    }
}