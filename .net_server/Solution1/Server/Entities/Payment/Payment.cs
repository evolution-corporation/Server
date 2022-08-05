namespace Server.Entities.Payment;

public class Payment
{
    public Payment()
    {
    }
    public Payment(int id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime PaymentDateTime { get; set; }

    public bool Confirm { get; set; } = false;
}