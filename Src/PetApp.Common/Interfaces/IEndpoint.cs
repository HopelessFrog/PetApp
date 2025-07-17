using Microsoft.AspNetCore.Routing;

namespace PetApp.Common.Interfaces;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}