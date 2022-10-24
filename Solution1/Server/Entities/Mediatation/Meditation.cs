namespace Server.Entities.Mediatation;

public class Meditation
{
    public Guid Id { get; set; }
    public string? Language { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TypeMeditation TypeMeditation { get; set; }
    
    public bool IsSubscribed { get; set; }
    public Guid? AudioId { get; set; }
    public int AudioLength { get; set; }
    public Guid? PhotoId { get; set; }
    public IList<UserMeditation> UserMeditations { get; set; }
}