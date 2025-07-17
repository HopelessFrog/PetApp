namespace PetApp.UserService.Models;

public class UserPreferences
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    
    public UserProfile UserProfile { get; set; } = null!;
}