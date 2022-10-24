using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface INotificationService
{
    public void SubUserNotification(string userToken, string expoToken, int notificationFrequency);

    public void UnsubUserNotification(string userToken);
}

public class NotificationService : INotificationService
{
    private DataContext Context;

    public NotificationService(DataContext context)
    {
        Context = context;
    }

    public void SubUserNotification(string userToken, string expoToken, int notificationFrequency)
    {
        var user = Context.GetUser(userToken);
        if (user == null)
            throw new NotSupportedException();
        var notification = Context.Notifications.AsQueryable().FirstOrDefault(x => x.UserId == user.Id);
        if (notification != null)
        {
            notification.IsSubscribedToNotification = true;
            Context.SaveChanges();
            return;
        }

        notification = new Notification(user.Id, expoToken, notificationFrequency)
        {
            IsSubscribedToNotification = true
        };
        Context.Notifications.Add(notification);
        Context.SaveChanges();
    }

    public void UnsubUserNotification(string userToken)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(userToken);
        task.Wait();
        var uid = task.Result.Uid;

        Context.Notifications.AsQueryable().First(x => x.UserId == uid).IsSubscribedToNotification = false;

        Context.SaveChanges();
    }
}