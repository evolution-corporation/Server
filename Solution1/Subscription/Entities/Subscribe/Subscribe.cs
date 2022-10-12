using System.ComponentModel.DataAnnotations;

namespace Subscription.Entities.Subscribe;

public class Subscribe
{
    [Key] public string UserId { get; set; }
    public DateTime WhenSubscribe { get; set; }
    public int RemainingTime { get; set; }
    public SubscribeType Type { get; set; }
    public int RebillId { get; set; } = -1;

    private Subscribe(string userId, int remaining, SubscribeType type)
    {
        UserId = userId;
        WhenSubscribe = DateTime.Today;
        RemainingTime = remaining;
        Type = type;
    }

    public Subscribe()
    {
    }

    public static Subscribe Convert(string userId, SubscribeType type)
    {
        return new Subscribe(userId, type switch
        {
            SubscribeType.Week => 7,
            SubscribeType.Month => 90,
            SubscribeType.Month6 => 180,
            _ => 0
        }, type);
    }
}