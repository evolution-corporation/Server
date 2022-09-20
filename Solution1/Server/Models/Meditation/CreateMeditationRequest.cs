using System.ComponentModel.DataAnnotations;
using Server.Entities;

namespace Server.Models.Meditation;

public class CreateMeditationRequest
{
    [Required]
#pragma warning disable CS8618
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
#pragma warning restore CS8618
    [Required]
    public TypeMeditation TypeMeditation { get; set; }
    [Required]
    public TimeMeditation TimeMeditation { get; set; }
    [Required]
    public CountDayMeditation CountDayMeditation { get; set; }
    [Required]
    public string? Language { get; set; }
    public MeditationSubscription? Subscription { get; set; }
    public string? SubscriptionPhoto { get; set; }   
    public string? MeditationPhoto { get; set; }
    
    public bool IsSubscribed { get; set; }
}