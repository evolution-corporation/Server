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
        var credential = (TinkoffCredential)JsonSerializer.Deserialize(File.ReadAllText("../settings/tinkoff_key.json"), typeof(TinkoffCredential))!; 
        container.Bind<TinkoffCredential>().ToConstant(credential);
        var resources = (Resources)JsonSerializer.Deserialize(File.ReadAllText("../settings/resources.json"),typeof(Resources))!;
        container.Bind<Resources>().ToConstant(resources);
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