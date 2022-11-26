using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Subscription.Entities.Payment;
using Subscription.Entities.Subscribe;
using Subscription.Entities.User;

namespace Subscription;

public sealed class Context : DbContext
{
    private readonly Resources resources;
    public DbSet<Subscribe> Subscribes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Payment> Payments;

    public Context(Resources resources)
    {
        this.resources = resources;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
       options.UseNpgsql(resources.DbConnectionString);
      }
}