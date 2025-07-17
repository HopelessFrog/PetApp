using Microsoft.EntityFrameworkCore;
using PetApp.UserService.Models;

namespace PetApp.UserService.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsRequired();
            
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(100);
            
            entity.Property(e => e.Location)
                .HasMaxLength(50);
        });
        
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();
            
            entity.HasOne(e => e.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(e => e.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(e => e.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<UserPreferences>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Key }).IsUnique();
            
            entity.HasOne(e => e.UserProfile)
                .WithMany(u => u.Preferences)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}