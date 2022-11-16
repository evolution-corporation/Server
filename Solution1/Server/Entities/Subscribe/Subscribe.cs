using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

public class Subscribe
{
    public Subscribe()
    {
    }

    public Subscribe(string userId, SubscribeType type)
    {
        UserId = userId;
        RemainingTime = SubscribeTypeConverter.GetSubscribeTime(type);
        Type = type;
    }

    [Key] public string UserId { get; set; }
    //public User User { get; set; }
    public DateTime WhenSubscribe { get; set; } = DateTime.Today;
    public int RemainingTime { get; set; }
    public SubscribeType Type { get; set; }
    public int RebillId { get; set; } = -1;
    
}