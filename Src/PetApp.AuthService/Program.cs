using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PetApp.AuthService.Data;
using PetApp.AuthService.Endpoints;
using PetApp.AuthService.Extensions;
using PetApp.AuthService.Jobs;
using PetApp.AuthService.Services;
using PetApp.Common.Extensions;
using PetApp.Common.Middleware;
using PetApp.Common.Models;
using Hangfire.MemoryStorage;
using PetApp.Common.Migrations;
using RefreshToken = PetApp.AuthService.Models.RefreshToken;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(nameof(JwtSettings)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.AddNpgsqlDbContext<AuthDbContext>("postgresdb");


builder.AddRedisClient("jti-ban");

builder.Services.AddJwtAuthentication();
builder.Services.AddHostedService<Migrator<AuthDbContext>>();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage());

builder.Services.AddHangfireServer();
builder.Services.AddScoped<TokenCleanupJob>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName!.Replace("+", "."));
    options.AddSecurity();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserInfo, HttpHeaderUserInfo>();

var app = builder.Build();
app.MapDefaultEndpoints();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
app.ConfigureTokenCleanupJob();

Singin.Map(app);
Singup.Map(app);
Refresh.Map(app);
Logout.Map(app);
ChangePassword.Map(app);

app.Run();