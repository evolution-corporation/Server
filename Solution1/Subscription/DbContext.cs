using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.Entities;
using Server.Entities.Payment;

namespace Subscription;

public sealed class Context : DbContext
{
    private readonly IConfiguration configuration;
    public DbSet<Subscribe> Subscribes { get; set; }
    public DbSet<User> Users { get; set; }

    public Context(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserMeditation>().HasKey(sc => new { sc.UserId, sc.MeditationId });
        modelBuilder.Entity<Notification>().HasKey(x => x.UserId);
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("WebApiDatabase"));
    }
}