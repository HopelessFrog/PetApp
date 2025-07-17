using System.Security.Claims;
using PetApp.AuthService.Models;

namespace PetApp.AuthService.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}