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
    
    public bool IsSubscribed { get; set; }
}