using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Models.Meditation;

public class CreateMeditationRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Image { get; set; }
    [Required]
    public TypeMeditation TypeMeditation { get; set; }
    [Required]
    public string Audio { get; set; }
    public bool IsSubscribed { get; set; }
}