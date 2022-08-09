using System.Dynamic;
using Expo.Server.Client;
using Expo.Server.Models;
using Microsoft.EntityFrameworkCore;
using Server.Helpers;

namespace Server.Services;

public static class PushService
{
    public static void Send(string token, string message)
    {
        var client = new PushApiClient
        {
            AccessToken = token
        };
        var pushTicketReq = new PushTicketRequest
        {
            PushTo = new List<string> { "..." },
            PushBadgeCount = 7,
            PushBody = message
        };
        var task = client.PushSendAsync(pushTicketReq);
        task.Wait();
        var result = task.Result;
        if (!(result.PushTicketErrors.Count > 0)) return;
        foreach (var error in result.PushTicketErrors)
        {
            Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
        }
    }
}

public class Notificator
{
    public DataContext context;

    public Notificator(DataContext context)
    {
        this.context = context;
        Task.Run(Invoke);
    }

    private async void Invoke()
    {
        var xyu = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 15, 0, 0);
        var timeToSleep = DateTime.Now.Hour < 15 ? xyu - DateTime.Now : DateTime.Now - xyu;
        Thread.Sleep(timeToSleep);
        while (true)
        {
            var notifications = context.Notifications.AsQueryable();
            foreach (var notification in notifications.Where(x => x.IsSubscribedToNotification &&
                                                                  DateTime.Today.Day - x.LastNotification.Day <=
                                                                  x.NotificationFrequency))
            {
                PushService.Send(notification.ExpoToken, "It's time to meditate");
            }
            Thread.Sleep(86400000);
        }
    }
}