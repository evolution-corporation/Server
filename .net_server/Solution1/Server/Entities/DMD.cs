namespace Server.Entities;

public class Dmd
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Guid> MeditationsId { get; set; }
}