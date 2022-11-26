namespace Server.Entities.Payment;

public class TinkoffNotification
{
    public string TerminalKey { get; set; }
    public string OrderId { get; set; }
    public bool Success { get; set; }
    public string Status { get; set; }
    public int PaymentId { get; set; }
    public string ErrorCode { get; set; }
    public int Amount { get; set; }
    public long RebillId { get; set; }
    public string Pan { get; set; }
    public string ExpDate { get; set; }
    public string Token { get; set; }
}