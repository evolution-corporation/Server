using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Subscription.Entities.Payment;
using Subscription.Entities.Subscribe;
using Subscription.Entities.User;

namespace Subscription;

public sealed class Context : DbContext
{
    private readonly IConfiguration configuration;
    public DbSet<Subscribe> Subscribes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Payment> Payments;

    public Context(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("WebApiDatabase"));
    }
}