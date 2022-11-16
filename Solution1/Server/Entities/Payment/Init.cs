using Server.Helpers;

namespace Server.Entities.Payment;

public class Init
{
    public Init(string terminalKey, int amount, string orderId, char recurrent, string customerKey, Resources resources)
    {
        TerminalKey = terminalKey;
        Amount = amount;
        OrderId = orderId;
        Recurrent = recurrent;
        CustomerKey = customerKey;
        NotificationURL = resources.MineIP;
    }

    public string TerminalKey { get; set; }
    public int Amount { get; set; }
    public string OrderId { get; set; }
    public char Recurrent { get; set; }
    public string CustomerKey { get; set; }
    
    public string NotificationURL { get; set; }
}

public class InitResponse
{
    public string TerminalKey { get; set; }
    public int Amount { get; set; }
    public string OrderId { get; set; }
    public bool Success { get; set; }
    public int PaymentId { get; set; }
    public string ErrorCode { get; set; }
    public string PaymentURL { get; set; }
}