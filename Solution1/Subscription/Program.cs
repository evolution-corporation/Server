using Microsoft.Extensions.Configuration;
using Ninject;
using Subscription;

static StandardKernel ConfigureContainer()
{
    var container = new StandardKernel();
    container.Bind<Context>().To<Context>();
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false);
    IConfiguration configuration = builder.Build();
    container.Bind<IConfiguration>().ToConstant(configuration);
    return container;
}

var container = ConfigureContainer();

var context = container.Get<Context>();

foreach (var subscribe in context.Subscribes)
{
    subscribe.RemainingTime--;
    if (subscribe.RemainingTime < 0)
    {
        context.Users.First(x => x.Id == subscribe.UserId).IsSubscribed = false;
    }
}
context.SaveChanges();