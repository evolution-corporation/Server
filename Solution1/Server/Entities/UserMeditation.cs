namespace Server.Entities;

public class UserMeditation
{
#pragma warning disable CS8618
    public UserMeditation(string userId,Guid meditationId, DateTime time)
#pragma warning restore CS8618
    {
        UserId = userId;
        MeditationId = meditationId;
        Time = time;
    }
    public int Id { get; set; } 
    public string UserId { get; set; }
    public User User { get; set; }
    
    public Guid MeditationId { get; set; }
    public Meditation Meditation { get; set; }
    public DateTime Time { get; set; }
}