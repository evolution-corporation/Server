namespace Server.Entities;

public class User
{
#pragma warning disable CS8618
    public string Id { get; set; }
#pragma warning restore CS8618
    public string? NickName { get; set; }
    public string? Birthday { get; set; }
    public string? DisplayName { get; set; }
    public string? Status { get; set; }
    public Role Role { get; set; }
    public UserGender Gender { get; set; }
    public UserCategory Category { get; set; }
    public DateTime DateTimeRegistration { get; set; } = DateTime.Now;
    public bool HasPhoto { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public IList<UserMeditation>? UserMeditations { get; set; }
    public int RebillId { get; set; } = -1;
    public bool IsSubscribed { get; set; }
}