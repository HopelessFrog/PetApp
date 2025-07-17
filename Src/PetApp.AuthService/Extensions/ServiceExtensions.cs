using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using PetApp.AuthService.Services;
using PetApp.AuthService.Services.Interfaces;
using PetApp.Common.Configurations;
using PetApp.Common.Extensions;
using FluentValidation;
using Hangfire;
using PetApp.AuthService.Jobs;

namespace PetApp.AuthService.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddJwt(true);

        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();

        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}