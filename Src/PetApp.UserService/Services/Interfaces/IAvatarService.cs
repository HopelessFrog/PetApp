namespace PetApp.UserService.Services.Interfaces;

public interface IAvatarService
{
    Task<string> UploadAvatarAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<string> GetAvatarUrlAsync(string fileName, CancellationToken cancellationToken = default);
}