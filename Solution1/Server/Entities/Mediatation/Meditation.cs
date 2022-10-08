namespace Server.Entities;

//TODO: Обнуление счётчика прослушиваний
public class Meditation
{
    public Guid id { get; set; } = Guid.NewGuid();
    public string? Language { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TypeMeditation TypeMeditation { get; set; }
    public TimeMeditation Time { get; set; }
    public bool IsSubscribed { get; set; }
    public bool HasAudio { get; set; }
    public int AudioLength { get; set; }
    public IList<UserMeditation> UserMeditations { get; set; }
}