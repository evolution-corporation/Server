using System.ComponentModel.DataAnnotations;
using Server.Entities;
using Server.Entities.Mediatation;

namespace Server.Models.Meditation;

public class CreateMeditationRequest
{
    [Required] public Guid Id { get; set; }
    [Required]
#pragma warning disable CS8618
    public string Name { get; set; }
    [Required] public string Description { get; set; }
#pragma warning restore CS8618
    [Required] public TypeMeditation TypeMeditation { get; set; }
    [Required] public string Language { get; set; }
    public int AudioLength { get; set; }
    public Guid AudioIds { get; set; }
    public Entities.Mediatation.Subscription? Subscription { get; set; }
    public string? SubscriptionPhoto { get; set; }
    public string? MeditationPhoto { get; set; }

    public bool IsSubscribed { get; set; }
}