using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Entities.Payment;
using Server.Helpers;
using JsonException = System.Text.Json.JsonException;

namespace Server.Services;

public interface IPaymentService
{
    public string SubscribeUser(string token, bool recurrentPayment, SubscribeType type);
}

public class PaymentService : IPaymentService
{
    private readonly DataContext context;
    private readonly TinkoffCredential credential;
    private int paymentId;
    private Payment payment;

    //TODO: Сделать проверку платежа от Тинькоффа
    public PaymentService(DataContext context, TinkoffCredential credential)
    {
        this.context = context;
        this.credential = credential;
    }

    public string SubscribeUser(string token, bool recurrentPayment, SubscribeType type)
    {
        var user = context.GetUser(token);
        if (user == null)
            throw new NotSupportedException();
        payment = new Payment(user.Id)
        {
            RecurrentPayment = recurrentPayment,
            Amount = SubcribeTypeConverter(type)
        };
        context.Payments.Add(payment);
        context.SaveChanges();
        var result = InitPayment(user.Id, recurrentPayment, SubcribeTypeConverter(type), payment.Id);
        // Task.Run(() => CheckPaymentResult(guid, type));
        return result;
    }

    private string InitPayment(string userId, bool recurrentPayment, int amount, string orderId)
    {
        var client = new HttpClient();
        var message = new HttpRequestMessage(HttpMethod.Post, "https://securepay.tinkoff.ru/v2/Init");
        var init = new Init(credential.TerminalKey, amount, orderId, recurrentPayment ? 'Y' : 'N', userId);
        message.Content = new StringContent(JsonSerializer.Serialize(init));
        using var answer = client.Send(message);
        var task = answer.Content.ReadFromJsonAsync<InitResponse>();
        task.Wait();
        var response = task.Result;
        if (response == null)
            throw new JsonException();
        paymentId = response.PaymentId;
        return response.PaymentURL;
    }

    private int SubcribeTypeConverter(SubscribeType type)
    {
        return type switch
        {
            SubscribeType.Week => 100,
            SubscribeType.Month => 47900,
            SubscribeType.Month6 => 199000,
            _ => throw new NotImplementedException()
        };
    }
}