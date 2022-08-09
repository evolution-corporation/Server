namespace Server.Entities;

public class Notification
{
    public Notification(string userId, string expoToken, int notificationFrequency)
    {
        UserId = userId;
        ExpoToken = expoToken;
        NotificationFrequency = notificationFrequency;
    }

    public string UserId { get; set; }
    public string ExpoToken { get; set; }
    public int NotificationFrequency { get; set; }
    public DateTime LastNotification { get; set; } = DateTime.Now;
    public bool IsSubscribedToNotification { get; set; }
}