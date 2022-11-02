using System.ComponentModel.DataAnnotations;

namespace Server.Entities.Mediatation;

public class Subscription
{
    [Key] public Guid Id { get; set; }
    public string Language { get; set; }
    public string Headers { get; set; }
    public string Description { get; set; }
    public string PayloadText { get; set; }
    //public Meditation Meditation { get; set; }
    public Guid SubscriptionPhotoId { get; set; }
}