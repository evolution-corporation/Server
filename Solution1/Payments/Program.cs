// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Ninject;
using Payments;

static StandardKernel ConfigureContainer()
{
    var container = new StandardKernel();
    container.Bind<Context>().To<Context>();
    var file = File.ReadLines("../Server/appsettings.json").ToArray();
    var connectionString = (ConnectionString)JsonSerializer.Deserialize($"{{{file[3]}}}", typeof(ConnectionString))!;
    container.Bind<ConnectionString>().ToMethod(_ => connectionString).InSingletonScope();
    return container;
}

static void CreateNewPayment()
{
    
}

var container = ConfigureContainer();
while (true)
{
    var context = container.Get<Context>();
    var client = new HttpClient();
    var message = new HttpRequestMessage(HttpMethod.Post,"	https://securepay.tinkoff.ru/v2/Charge");
    var subscribes = context.Subscribes;
    foreach (var sub in subscribes)
    {
        if (sub.RemainingTime == 0)
        {
            
        }
    }
}