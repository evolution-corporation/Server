using Microsoft.Extensions.Caching.Memory;
using Server.Entities;
using Server.Entities.Payment;

namespace Server.Helpers;

using Microsoft.EntityFrameworkCore;
using Entities;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;
    public DbSet<User> Users { get; set; }
    public DbSet<Meditation> Meditations { get; set; }
    public DbSet<Subscribe> Subscribes { get; set; }
    public DbSet<Dmd> DMDs { get; set; }
    public DbSet<Payment> Payments { get; set; }

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // in memory database used for simplicity, change to a real db for production applications
        options.UseMemoryCache(new MemoryCache(new MemoryCacheOptions()));
        //options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
    }
}