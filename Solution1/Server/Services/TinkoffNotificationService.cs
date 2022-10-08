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

    public TinkoffNotificationService(DataContext context)
    {
        this.context = context;
    }

    public void CheckPayment(TinkoffNotification notification)
    {
        var payment = context.Payments.First(x => x.Id == notification.OrderId);
        if (!notification.Success) return;
        payment.Confirm = true;
        if (payment.RecurrentPayment) 
            context.Subscribes.First(x => x.UserId == payment.UserId).RebillId = notification.RebillId;
        context.SaveChanges();
    }
}