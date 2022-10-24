namespace Subscription.Entities.Payment;

public class Payment
{
    public Payment()
    {
    }
    public Payment(string userId)
    {
        UserId = userId;
    }
    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public int Amount { get; set; }
    public DateTime PaymentDateTime { get; set; } = DateTime.Now;
    public bool RecurrentPayment { get; set; }
    public bool Confirm { get; set; } = false;
}