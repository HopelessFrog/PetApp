using PetApp.AuthService.Models;

namespace PetApp.AuthService.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetValidRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    
    Task RevokeRefreshAllTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}