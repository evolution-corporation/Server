using FirebaseAdmin.Auth;
using Microsoft.Extensions.Caching.Memory;
using Server.Entities;
using Server.Entities.Mediatation;
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
    public DbSet<Entities.Mediatation.Subscription> MeditationSubscriptions { get; set; }

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
        modelBuilder.Entity<Meditation>().HasMany<UserMeditation>();
        modelBuilder.Entity<User>().HasMany<UserMeditation>();
        modelBuilder.Entity<UserMeditation>().HasOne<User>().WithMany(x => x.UserMeditations)
            .HasForeignKey(x => x.User);
        modelBuilder.Entity<UserMeditation>().HasOne<Meditation>().WithMany(x => x.UserMeditations)
            .HasForeignKey(x => new { x.Meditation, x.Language });
        modelBuilder.Entity<UserMeditation>()
            .HasKey(sc => new { UserId = sc.User, MeditationId = sc.Meditation, sc.Language });
        //modelBuilder.Entity<Notification>().HasKey(x => x.UserId);
        //modelBuilder.Entity<Entities.Mediatation.Subscription>().HasOne<Meditation>().WithOne(x => x.Subscription)
        //    .HasForeignKey<Meditation>(x => new { x.Id, x.Language });
        modelBuilder.Entity<Entities.Mediatation.Subscription>().HasKey(x => new { x.Id, x.Language });
        modelBuilder.Entity<Meditation>().HasKey(x => new { x.Id, x.Language });
        //modelBuilder.Entity<Subscribe>().HasOne<User>().WithOne(x => x.Subscribe).HasForeignKey<User>(x => x.Id);
        //modelBuilder.Entity<Subscribe>().HasKey(x => x.UserId);
        //modelBuilder.Entity<User>().HasOne<Subscribe>().WithOne(x => x.User);
        //modelBuilder.Entity<Subscribe>().HasKey(x => x.Id);
        base.OnModelCreating(modelBuilder);
    }

    public User? GetUser(string token)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        return Users.FirstOrDefault(x => x.Id.Equals(task.Result.Uid));
    }
}