using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Ninject;
using Payments;

namespace Subscription;

public class CheckSubscription
{
    static StandardKernel ConfigureContainer()
    {
        var container = new StandardKernel();
        container.Bind<Context>().To<Context>();
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
        IConfiguration configuration = builder.Build();
        container.Bind<IConfiguration>().ToConstant(configuration);
        var credential = (TinkoffCredential)JsonSerializer.Deserialize(
            configuration.GetSection("TinkoffCredential").Value, typeof(TinkoffCredential))!;
        container.Bind<TinkoffCredential>().ToConstant(credential);
        return container;
    }

    public void Run()
    {
        var container = ConfigureContainer();
        var context = container.Get<Context>();
        foreach (var subscribe in context.Subscribes)
        {
            if (subscribe.WhenSubscribe + new TimeSpan(subscribe.RemainingTime, 0, 0, 0) < DateTime.Now)
            {
                context.Users.First(x => x.Id == subscribe.UserId).IsSubscribed =
                    RebillSubscription.RebillSubscribe(subscribe, context, container.Get<TinkoffCredential>());
            }
        }
        Thread.Sleep(86400000);
        context.SaveChanges();
    }
}