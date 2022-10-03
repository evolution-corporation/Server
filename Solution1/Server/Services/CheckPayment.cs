namespace Server.Entities;

public class CheckPayment
{
    public CheckPayment(string terminalKey, int orderId, string token)
    {
        TerminalKey = terminalKey;
        OrderId = orderId;
        Token = token;
    }
    public string TerminalKey { get; set; }
    public int OrderId { get; set; }
    public string Token { get; set; }
}