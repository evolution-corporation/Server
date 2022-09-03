using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.Entities;
using Server.Entities.Payment;

namespace Payments;

public sealed class Context : DbContext
{
    private readonly ConnectionString connectionString;
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Subscribe> Subscribes { get; set; }

    public Context(ConnectionString connectionString)
    {
        this.connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString.WebApiDatabase);
    }
}

public class ConnectionString
{
    public readonly string WebApiDatabase;

    public ConnectionString(string webApiDatabase)
    {
        WebApiDatabase = webApiDatabase;
    }

    public ConnectionString()
    {
    }
}