
using System.Security.Cryptography;

namespace Server.Entities;

public class CheckPayment
{
    public CheckPayment(string terminalKey, int orderId)
    {
        TerminalKey = terminalKey;
        OrderId = orderId;
        var s = ""; //orderid, password,terminalkey
        var x  = SHA256.Create().ComputeHash(s.Select(x => (byte)x).ToArray());
    }

    public string TerminalKey { get; } = "1635503669838DEMO";
    public int OrderId { get; }
    public string sha256 { get; }
}