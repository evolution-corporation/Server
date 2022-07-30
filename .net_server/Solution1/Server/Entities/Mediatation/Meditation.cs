namespace Server.Entities;

public class Meditation
{
    private static int IdCount;

    static Meditation() => IdCount = 1;
    public int id { get; set; } = IdCount++;
    public string Language { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public TypeMeditation TypeMeditation { get; set; }
    public bool IsSubscribed { get; set; }
    public int ListenedToday { get; set; }
}