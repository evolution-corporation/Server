using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FirebaseAdmin.Auth;
using Newtonsoft.Json;
using Server.Entities;
using Server.Entities.Payment;
using Server.Helpers;

namespace Server.Services;

public interface IPaymentService
{
    public int GenerateUniqueId(string token);
    public void ConfirmPayment(string token, int id);
}

public class PaymentService : IPaymentService
{
    private readonly DataContext context;
    private readonly TinkoffCredential credential;

    //TODO: Сделать проверку платежа от Тинькоффа
    public PaymentService(DataContext context, TinkoffCredential credential)
    {
        this.context = context;
        this.credential = credential;
    }

    public int GenerateUniqueId(string token)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        var guid = task.Result.Uid;
        var payment = new Payment(guid);
        context.Payments.Add(payment);
        context.SaveChanges();
        return payment.Id;
    }

    public void ConfirmPayment(string token, int id)
    {
        var send = new Func<object?>(() =>
        {
            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Post, "https://securepay.tinkoff.ru/v2/CheckOrder");
            var check = new CheckPayment(credential.TerminalKey, id, GenerateToken(id));
            message.Content = new StringContent(JsonConvert.SerializeObject(check));
            var answer = client.Send(message);
            
            return false;
        });

        Thread.Sleep(10000);
        FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        context.Payments.AsQueryable().First(x => x.Id == id).Confirm = true;
    }

    private string GenerateToken(int orderId)
    {
        //orderid, password,terminalkey
        using var hash = SHA256.Create();
        return string.Concat(hash
            .ComputeHash(Encoding.UTF8.GetBytes($"{orderId}{credential.Password}{credential.TerminalKey}"))
            .Select(item => item.ToString("x2")));
    }
}