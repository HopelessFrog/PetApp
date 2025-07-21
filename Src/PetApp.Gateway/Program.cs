using Microsoft.Extensions.Options;
using PetApp.Common.Extensions;
using PetApp.Common.Models;
using PetApp.Common.Models.Options;
using PetApp.Gateway.Configs;
using PetApp.Gateway.Extensions;
using PetApp.Gateway.Middleware;
using PetApp.Gateway.Transforms;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Swagger;
using Yarp.ReverseProxy.Swagger.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(nameof(JwtSettings)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.AddRedisClient("jti-ban");

builder.Services.AddJwt(true);


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuth", policy =>
        policy.RequireAuthenticatedUser());
});


var configuration = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy()
    .LoadFromConfig(configuration)
    .AddServiceDiscoveryDestinationResolver()
    .AddSwagger(configuration)
    .AddTransforms<AllClaimsTransform>();
;

builder.Services.AddServiceDiscovery();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
        options.ConfigureSwaggerEndpoints(config);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();