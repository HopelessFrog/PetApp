using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using PetApp.AuthService.Data;
using PetApp.AuthService.Models;
using PetApp.AuthService.Services.Interfaces;
using PetApp.Common.Extensions;
using PetApp.Common.HttpResults;
using PetApp.Common.Interfaces;
using PetApp.Common.Models;

namespace PetApp.AuthService.Endpoints;

public class Singup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/auth/signup", Handle)
        .WithSummary("Creates a new user account")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Email, string Password);

    public record Response(string AccessToken, string RefreshToken);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }

    private static async Task<Results<Ok<Response>, ValidationErrorHttpResult>> Handle(
        Request request,
        AuthDbContext database,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        CancellationToken cancellationToken)
    {
        if (await database.Users
                .AnyAsync(x => x.Username == request.Username, cancellationToken))
        {
            return new ValidationErrorHttpResult("Username is already taken");
        }


        if (await database.Users
                .AnyAsync(x => x.Email == request.Email, cancellationToken))
        {
            return new ValidationErrorHttpResult("Email is already registered");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.Password.GetSha256Hash(),
        };

        await database.Users.AddAsync(user, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id.ToString(), cancellationToken);

        var response = new Response(accessToken, refreshToken.Token);
        return TypedResults.Ok(response);
    }
}