using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using PetApp.AuthService.Data;
using PetApp.AuthService.Models;
using PetApp.AuthService.Services.Interfaces;
using PetApp.Common.Extensions;
using PetApp.Common.Interfaces;
using PetApp.Common.Models;

namespace PetApp.AuthService.Endpoints;

public class Singin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/auth/signin", Handle)
        .WithSummary("Logs in a user")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password);

    public record Response(string AccessToken, string RefreshToken);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
        Request request,
        AuthDbContext database,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        CancellationToken cancellationToken)
    {
        var user = await database.Users
            .FirstOrDefaultAsync(
                x => (x.Username == request.Username || x.Email == request.Username) &&
                     x.PasswordHash == request.Password.GetSha256Hash(), cancellationToken);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        await database.SaveChangesAsync(cancellationToken);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id.ToString(), cancellationToken);

        var response = new Response(accessToken, refreshToken.Token);
        return TypedResults.Ok(response);
    }
}