namespace Subscription.Entities.User;

public class User
{
#pragma warning disable CS8618
    public string Id { get; set; }
#pragma warning restore CS8618
    public string? NickName { get; set; }
    public DateTime? Birthday { get; set; }
    public string? DisplayName { get; set; }
    public Role Role { get; set; }
    public UserGender Gender { get; set; }
    public DateTime DateTimeRegistration { get; set; } = DateTime.Now;
    public bool HasPhoto { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    // public int RebillId { get; set; } = -1;
    public bool IsSubscribed { get; set; }
}