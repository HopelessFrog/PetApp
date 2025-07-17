using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PetApp.Common.HttpResults;
using PetApp.Common.Interfaces;
using PetApp.Common.Models;
using PetApp.UserService.Data;

namespace PetApp.UserService.Endpoints.Profile;

public class GetMyProfile : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/profile", Handle)
        .WithSummary("Get auntificated user profile");

    public record Response(string Username, string Description, string AvatarUrl, string Location);

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        UserDbContext database,
        IUserInfo userInfo,
        CancellationToken cancellationToken)
    {
        var user = await database.UserProfiles
            .Where(user => user.Id.ToString() == userInfo.Id)
            .Select(user => new Response(user.Username, user.Description, user.AvatarUrl, user.Location))
            .FirstOrDefaultAsync();

        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(user);
    }
}