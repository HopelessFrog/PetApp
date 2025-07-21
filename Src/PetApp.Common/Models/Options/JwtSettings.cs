namespace PetApp.Common.Models.Options;

public class JwtSettings
{
    public required string SecretKey { get; set; }
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}