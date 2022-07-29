using WebApi.Entities;

namespace WebApi.Models.Meditation;

public class UpdateMeditationRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public TypeMeditation TypeMeditation { get; set; }
    public string Audio { get; set; }
    public bool IsSubscribed { get; set; }
}