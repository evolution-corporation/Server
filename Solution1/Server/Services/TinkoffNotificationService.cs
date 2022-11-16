using System.Security.Authentication;
using Server.Entities;
using Server.Entities.Payment;
using Server.Helpers;

namespace Server.Services;

public interface ITinkoffNotificationService
{
    public void CheckPayment(TinkoffNotification notification);
}

public class TinkoffNotificationService : ITinkoffNotificationService
{
    private readonly DataContext context;
    private readonly TinkoffCredential _credential;

    public TinkoffNotificationService(DataContext context, TinkoffCredential credential)
    {
        this.context = context;
        _credential = credential;
    }

    public void CheckPayment(TinkoffNotification notification)
    {
        if (!string.Equals(notification.TerminalKey, _credential.TerminalKey, StringComparison.InvariantCulture))
            throw new AuthenticationException();
        var payment = context.Payments.First(x => x.Id == notification.OrderId);
        if (!notification.Success) return;
        payment.Confirm = true;
        var subscribe = new Subscribe(payment.UserId,payment.SubscribeType);
        if (payment.RecurrentPayment) 
            subscribe.RebillId = notification.RebillId;
        context.Subscribes.Add(subscribe);
        context.SaveChanges();
    }
}