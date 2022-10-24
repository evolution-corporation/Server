using System.ComponentModel.DataAnnotations;

namespace Server.Entities.Mediatation;

public class Subscription
{
    [Key]
    public Guid MeditationId { get; set; }
    public string Headers { get; set; }
    public string Description { get; set; }
    public string PayloadText { get; set; }
}