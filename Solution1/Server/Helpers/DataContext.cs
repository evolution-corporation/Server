using FirebaseAdmin.Auth;
using Microsoft.Extensions.Caching.Memory;
using Server.Entities;
using Server.Entities.Payment;

namespace Server.Helpers;

using Microsoft.EntityFrameworkCore;
using Entities;

public class DataContext : DbContext
{
    private readonly IConfiguration Configuration;
    public DbSet<User> Users { get; set; }
    public DbSet<Meditation> Meditations { get; set; }
    public DbSet<Subscribe> Subscribes { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<UserMeditation> UserMeditations { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Subscription> MeditationSubscriptions { get; set; }

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        //options.UseMemoryCache(new MemoryCache(new MemoryCacheOptions()));
        options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserMeditation>().HasKey(sc => new { sc.UserId, sc.MeditationId });
        modelBuilder.Entity<Notification>().HasKey(x => x.UserId);
        base.OnModelCreating(modelBuilder);
    }

    public string GetUserId(string token)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        return task.Result.Uid;
    }
}