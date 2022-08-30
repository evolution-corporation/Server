using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

public class MeditationSubscription
{
    [Key]
    public int MeditationId { get; set; }
    public string Headers { get; set; }
    public string Subscription { get; set; }
    public string PayloadText { get; set; }
}