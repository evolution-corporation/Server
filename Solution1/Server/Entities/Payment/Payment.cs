namespace Server.Entities.Payment;

public class Payment
{
    public Payment()
    {
    }
    public Payment(string userId)
    {
        UserId = userId;
    }
    
    public int Id { get; set; } = Guid.NewGuid().GetHashCode();
    public string UserId { get; set; }
    public DateTime PaymentDateTime { get; set; }

    public bool Confirm { get; set; } = false;
}