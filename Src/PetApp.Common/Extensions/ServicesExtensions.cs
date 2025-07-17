using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PetApp.Common.Configurations;
using PetApp.Common.Services;
using PetApp.Common.Services.Interfaces;

namespace PetApp.Common.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services, bool withAuth = false)
    {
        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        services.AddTransient<ITokenBlacklistService, TokenBlacklistService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        if (withAuth) services.AddAuthorization();

        return services;
    }
}