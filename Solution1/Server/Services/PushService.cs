using Expo.Server.Client;
using Expo.Server.Models;

namespace Server.Services;

public static class PushService
{
    public static void Send(string token, string message)
    {
        var client = new PushApiClient
        {
            AccessToken = token
        };
        var pushTicketReq = new PushTicketRequest {
            PushTo = new List<string> { "..." },
            PushBadgeCount = 7,
            PushBody = message
        };
        var result = client.PushSendAsync(pushTicketReq).Result;
        if (!(result.PushTicketErrors.Count > 0)) return;
        foreach (var error in result.PushTicketErrors) 
        {
            Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
        }
    }
}