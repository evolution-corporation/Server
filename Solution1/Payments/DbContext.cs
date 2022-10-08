using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Payments.Entities.Payment;
using Subscribe = Payments.Entities.Subscribe.Subscribe;

namespace Payments;

public sealed class Context : DbContext
{
    private readonly IConfiguration configuration;
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Subscribe> Subscribes { get; set; }

    public Context(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("WebApiDatabase"));
    }
}
