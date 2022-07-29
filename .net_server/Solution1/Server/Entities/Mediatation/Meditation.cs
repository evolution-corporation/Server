namespace WebApi.Entities;

public class Meditation
{
    public int id { get; set; }
    public string Language { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public TypeMeditation TypeMeditation { get; set; }
    public string Audio { get; set; }
    public bool IsSubscribed { get; set; }
    public int ListenedToday { get; set; }
}