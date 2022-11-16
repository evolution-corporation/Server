using System.Net;
using System.Net.Http.Headers;
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
    private Resources Resources;

    public PaymentService(DataContext context, TinkoffCredential credential, Resources resources)
    {
        this.context = context;
        this.credential = credential;
        Resources = resources;
    }

    public string SubscribeUser(string token, bool recurrentPayment, SubscribeType type)
    {
        var user = context.GetUser(token);
        if (user == null)
            throw new NotSupportedException();
        payment = new Payment(user.Id, type)
        {
            RecurrentPayment = recurrentPayment,
            Amount = SubscribeTypeConverter.GetSubscribePrice(type)
        };
        context.Payments.Add(payment);
        context.SaveChanges();
        var result = InitPayment(user.Id, recurrentPayment, payment.Amount, payment.Id);
        return result;
    }

    private string InitPayment(string userId, bool recurrentPayment, int amount, string orderId)
    {
        var client = new HttpClient();
        var message = new HttpRequestMessage(HttpMethod.Post, "https://securepay.tinkoff.ru/v2/Init");
        var init = new Init(credential.TerminalKey, amount, orderId, recurrentPayment ? 'Y' : 'N', userId, Resources);
        message.Content = new StringContent(JsonSerializer.Serialize(init));
        message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        using var answer = client.Send(message);
        var task = answer.Content.ReadFromJsonAsync<InitResponse>();
        task.Wait();
        var response = task.Result;
        if (response == null)
            throw new JsonException();
        paymentId = response.PaymentId;
        return response.PaymentURL;
    }
}