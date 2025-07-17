namespace PetApp.UserService.Models;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public UserProfile Follower { get; set; } = null!;
    public UserProfile Following { get; set; } = null!;
}