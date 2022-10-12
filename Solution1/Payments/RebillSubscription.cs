using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Ninject;
using Payments;
using Payments.Entities.Payment;
using Payments.Entities.Subscribe;

namespace Payments;

public class RebillSubscription
{
    static StandardKernel ConfigureContainer()
    {
        var container = new StandardKernel();
        container.Bind<Context>().To<Context>();
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
        IConfiguration configuration = builder.Build();
        container.Bind<IConfiguration>().ToConstant(configuration);
        var str = File.ReadAllText(configuration.GetSection("TinkoffCredential").Value);
        var credential = (TinkoffCredential)JsonSerializer.Deserialize(str, typeof(TinkoffCredential))!;
        container.Bind<TinkoffCredential>().ToConstant(credential);
        return container;
    }

    static InitResponse InitPayment(TinkoffCredential credential, string userId, bool recurrentPayment, int amount,
        string orderId, HttpClient client)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "https://securepay.tinkoff.ru/v2/Init");
        var init = new Init(credential.TerminalKey, amount, orderId, recurrentPayment ? 'Y' : 'N', userId);
        message.Content = new StringContent(JsonSerializer.Serialize(init));
        using var answer = client.Send(message);
        var task = answer.Content.ReadFromJsonAsync<InitResponse>();
        task.Wait();
        var response = task.Result;
        if (response == null)
            throw new JsonException();
        return response;
    }

    static ChargeResponse ChargePayment(TinkoffCredential credential, InitResponse initResponse, int RebillId,
        HttpClient client)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "https://securepay.tinkoff.ru/v2/Charge");
        var charge = new Charge(credential.TerminalKey, initResponse.PaymentId, RebillId, credential.Password);
        message.Content = new StringContent(JsonSerializer.Serialize(charge));
        using var answer = client.Send(message);
        var task = answer.Content.ReadFromJsonAsync<ChargeResponse>();
        task.Wait();
        var response = task.Result;
        if (response == null)
            throw new NullReferenceException();
        return response;
    }

    static int SubcribeTypeConverter(SubscribeType type)
    {
        return type switch
        {
            SubscribeType.Week => 100,
            SubscribeType.Month => 47900,
            SubscribeType.Month6 => 199000,
            _ => throw new NotImplementedException()
        };
    }

    public void Run()
    {
        var container = ConfigureContainer();
        while(true)
        {
            var context = container.Get<Context>();
            var subscribes = context.Subscribes;
            var credential = container.Get<TinkoffCredential>();
            var client = new HttpClient();
            foreach (var subscribe in subscribes)
            {
                var rebillId = subscribe.RebillId;
                if (rebillId == -1)
                    continue;
                if (subscribe.RemainingTime == 1)
                {
                    var payment = new Payment(subscribe.UserId)
                    {
                        Amount = SubcribeTypeConverter(subscribe.Type)
                    };
                    var response = InitPayment(credential, subscribe.UserId, false,
                        SubcribeTypeConverter(subscribe.Type),
                        payment.Id, client);
                    var result = ChargePayment(credential, response, rebillId, client);
                    if (result.Success)
                    {
                        context.Payments.Add(payment);
                        subscribe.RemainingTime += Subscribe.Convert(subscribe.UserId, subscribe.Type).RemainingTime;
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
