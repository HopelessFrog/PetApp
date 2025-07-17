using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens.Experimental;
using PetApp.AuthService.Data;
using PetApp.AuthService.Extensions;
using PetApp.AuthService.Services.Interfaces;
using PetApp.Common.Extensions;
using PetApp.Common.Interfaces;
using PetApp.Common.Models;
using PetApp.Common.Services.Interfaces;

namespace PetApp.AuthService.Endpoints;

public class Refresh : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/auth/refresh", Handle)
        .WithSummary("Refreshes access token")
        .WithRequestValidation<Request>();

    public record Request(string AccessToken, string RefreshToken);

    public record Response(string AccessToken, string RefreshToken);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
        Request request,
        AuthDbContext database,
        IJwtService jwtService,
        ITokenBlacklistService blacklistService,
        IRefreshTokenService refreshTokenService,
        CancellationToken cancellationToken)
    {
        var claims = jwtService.GetPrincipalFromExpiredToken(request.AccessToken);

        var storedRefreshToken =
            await refreshTokenService.GetValidRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (storedRefreshToken is null ||
            storedRefreshToken.UserId.ToString() != claims?.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return TypedResults.Unauthorized();
        }

        storedRefreshToken.IsRevoked = true;

        var remainingTime = claims.UntilExpiration() ?? TimeSpan.Zero;
        if (remainingTime > TimeSpan.Zero)
        {
            await blacklistService.BlacklistTokenAsync(claims?.FindFirstValue(JwtRegisteredClaimNames.Jti),
                remainingTime);
        }

        await database.SaveChangesAsync(cancellationToken);
        
        var user = storedRefreshToken.User;
        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken =
            await refreshTokenService.CreateRefreshTokenAsync(storedRefreshToken.UserId.ToString(), cancellationToken);

        var response = new Response(newAccessToken, newRefreshToken.Token);
        return TypedResults.Ok(response);
    }
}