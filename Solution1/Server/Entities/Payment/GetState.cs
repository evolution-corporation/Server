using System.Security.Cryptography;
using System.Text;

namespace Server.Entities.Payment;

public class GetState
{
    public GetState(string terminalKey, int paymentId, string password)
    {
        TerminalKey = terminalKey;
        PaymentId = paymentId;
        using var hash = SHA256.Create();
        Token = string.Concat(hash
            .ComputeHash(Encoding.UTF8.GetBytes($"{paymentId}{password}{terminalKey}"))
            .Select(item => item.ToString("x2")));
    }

    public string TerminalKey { get; set; }
    public int PaymentId { get; set; }
    public string Token { get; set; }
}

public class GetStateResponse
{
    public string TerminalKey { get; set; }
    public string OrderId { get; set; }
    public bool Success { get; set; }
    public string Status { get; set; }
    public int PaymentId { get; set; }
    public string ErrorCode { get; set; }
}