namespace Server.Entities;

//TODO: Обнуление счётчика прослушиваний
public class Meditation
{
    public int id { get; set; } = Math.Abs(Guid.NewGuid().GetHashCode());
    public string? Language { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TypeMeditation TypeMeditation { get; set; }
    public CountDayMeditation CountDay { get; set; }
    public TimeMeditation Time { get; set; }
    public bool IsSubscribed { get; set; }
    public IList<UserMeditation> UserMeditations { get; set; }
}