using FirebaseAdmin.Auth;
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
    private static int index;
    private static bool isConfigured;

    static PaymentService() => isConfigured = false;

    public PaymentService(DataContext context) => this.context = context;

    public int GenerateUniqueId(string token)
    {
        var guid = new Guid(FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid);
        if(!isConfigured)
            Configure();
        var i = index++;
        context.Payments.Add(new Payment(i,guid));
        context.SaveChanges();
        return i;
    }

    public void ConfirmPayment(string token, int id)
    {
        var send = new Func<object?>(() =>
        {
            
            return false;
        });
        
        Thread.Sleep(10000);
        FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        context.Payments.AsQueryable().First(x => x.Id == id).Confirm = true;
    }

    private void Configure()
    {
        isConfigured = true;
        index = context.Payments.AsQueryable().MaxBy(x => x.Id)?.Id + 1 ?? 0;
    }
}