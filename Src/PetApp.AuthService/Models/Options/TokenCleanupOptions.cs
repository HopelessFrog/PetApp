namespace PetApp.AuthService.Models.Options;

public class TokenCleanupOptions
{
    public required string JobId { get; set; }
    public required string CronExpression { get; set; }
    
}