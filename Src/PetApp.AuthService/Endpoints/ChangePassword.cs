using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PetApp.AuthService.Data;
using PetApp.AuthService.Services.Interfaces;
using PetApp.Common.Extensions;
using PetApp.Common.HttpResults;
using PetApp.Common.Interfaces;
using PetApp.Common.Models;

namespace PetApp.AuthService.Endpoints;

public class ChangePassword : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/auth/changepass", Handle)
        .WithSummary("Change password")
        .WithRequestValidation<Request>();

    public record Request(string Password);
    
    public record Response(string RefreshToken);
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult,ValidationErrorHttpResult>> Handle(
        Request request,
        AuthDbContext database,
        IUserInfo userInfo,
        IRefreshTokenService refreshTokenService,
        CancellationToken cancellationToken)
    {
        var user = await database.Users
            .FirstOrDefaultAsync(user => user.Id.ToString() == userInfo.Id);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        if (user.PasswordHash == request.Password.GetSha256Hash())
        {
            return new ValidationErrorHttpResult("New password cannot be same as old");
        }
        
        user.PasswordHash = request.Password.GetSha256Hash();
        
        await database.SaveChangesAsync(cancellationToken);
        
        await refreshTokenService.RevokeRefreshAllTokensAsync(user.Id, cancellationToken);
        
        var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id.ToString(), cancellationToken);

        var response = new Response(refreshToken.Token);
        return TypedResults.Ok(response);
    }
}