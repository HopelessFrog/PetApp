using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PetApp.AuthService.Data;
using PetApp.AuthService.Models;
using PetApp.AuthService.Services.Interfaces;
using PetApp.Common.Models;
using PetApp.Common.Models.Options;

namespace PetApp.AuthService.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _settings;

    public RefreshTokenService(AuthDbContext context, IJwtService jwtService, IOptionsSnapshot<JwtSettings> settings)
    {
        _context = context;
        _jwtService = jwtService;
        _settings = settings.Value;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = new RefreshToken
        {
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays),
            UserId = Guid.Parse(userId)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token,
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token &&
                                       !rt.IsRevoked &&
                                       rt.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeRefreshAllTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync(user => user.Id == userId);
        user?.RefreshTokens.ForEach(rt => rt.IsRevoked = true);
        await _context.SaveChangesAsync(cancellationToken);
    }
}