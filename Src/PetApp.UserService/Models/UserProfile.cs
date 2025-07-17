namespace PetApp.UserService.Models;

public class UserProfile
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Location { get; set; }
    
    public ICollection<Subscription> Following { get; set; } = new List<Subscription>();
    public ICollection<Subscription> Followers { get; set; } = new List<Subscription>();
    public ICollection<UserPreferences> Preferences { get; set; } = new List<UserPreferences>();
}