using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using PetApp.Common.Extensions;
using PetApp.Common.Services;
using PetApp.Common.Services.Interfaces;

namespace PetApp.Gateway.Middleware;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBlacklistService _blacklistService;

    public TokenBlacklistMiddleware(RequestDelegate next, ITokenBlacklistService blacklistService)
    {
        _next = next;
        _blacklistService = blacklistService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var requiresAuthorization = endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;


        if (context.User.Identity?.IsAuthenticated == true && requiresAuthorization)
        {
            var jti = context.User.FindFirstValue(JwtRegisteredClaimNames.Jti);


            if (!string.IsNullOrEmpty(jti) && await _blacklistService.IsTokenBlacklistedAsync(jti))
            {
                context.Response.StatusCode = 401;
                return;
            }
        }

        await _next(context);
    }
}