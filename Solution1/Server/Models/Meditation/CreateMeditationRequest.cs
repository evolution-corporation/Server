using System.ComponentModel.DataAnnotations;
using Server.Entities;

namespace Server.Models.Meditation;

public class CreateMeditationRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public TypeMeditation TypeMeditation { get; set; }
    [Required]
    public TimeMeditation TimeMeditation { get; set; }
    [Required]
    public CountDayMeditation CountDayMeditation { get; set; }
    public MeditationSubscription? Subscription { get; set; }
    public string language { get; set; }
    public string? SubscriptionPhoto { get; set; }   
    public string? MeditationPhoto { get; set; }
    
    public bool IsSubscribed { get; set; }
}