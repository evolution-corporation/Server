using System.Security.Cryptography;
using System.Text;

namespace Server.Entities.Payment;

public class Charge
{
    public Charge(string terminalKey, int paymentId, int rebillId,string password)
    {
        TerminalKey = terminalKey;
        PaymentId = paymentId;
        RebillId = rebillId;
        using var hash = SHA256.Create();
        Token = string.Concat(hash
            .ComputeHash(Encoding.UTF8.GetBytes($"{password}{paymentId}{rebillId}{terminalKey}"))
            .Select(item => item.ToString("x2")));
    }
    public string TerminalKey { get; set; }
    public int PaymentId { get; set; }
    public int RebillId { get; set; }
    public string Token { get; set; }
}

public class ChargeResponse
{
    public string TerminalKey { get; set; }
    public int Amount { get; set; }
    public string OrderId { get; set; }
    public bool Success { get; set; }
    public string Status { get; set; }
    public int PaymentId { get; set; }
    public string ErrorCode { get; set; }
}