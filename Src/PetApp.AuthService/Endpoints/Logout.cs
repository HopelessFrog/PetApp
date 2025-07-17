using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PetApp.AuthService.Data;
using PetApp.AuthService.Services.Interfaces;
using PetApp.Common.Extensions;
using PetApp.Common.Interfaces;
using PetApp.Common.Models;
using PetApp.Common.Services;
using PetApp.Common.Services.Interfaces;

namespace PetApp.AuthService.Endpoints;

public class Logout : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/auth/logout", Handle)
        .WithSummary("Logs out user and revokes refresh token")
        .WithRequestValidation<Request>();

    public record Request(string RefreshToken);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
        }
    }

    private static async Task<Results<Ok, UnauthorizedHttpResult>> Handle(
        Request request,
        AuthDbContext database,
        IRefreshTokenService refreshTokenService,
        ITokenBlacklistService blacklistService,
        IUserInfo user,
        IOptionsSnapshot<JwtSettings> jwtSetting,
        CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenService.GetValidRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is not null && refreshToken.UserId.ToString() == user.Id)
        {
            await refreshTokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
            await blacklistService.BlacklistTokenAsync(user.Jti,
                user.Exp ?? TimeSpan.FromMinutes(jwtSetting.Value.AccessTokenExpirationMinutes));
        }

        return TypedResults.Ok();
    }
}