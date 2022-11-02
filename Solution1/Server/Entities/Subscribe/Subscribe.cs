using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

public class Subscribe
{
    [Key] public string UserId { get; set; }
    //public User User { get; set; }
    public DateTime WhenSubscribe { get; set; }
    public int RemainingTime { get; set; }
    public SubscribeType Type { get; set; }
    public int RebillId { get; set; } = -1;
}