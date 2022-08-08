namespace Server.Entities;

public class UserMeditation
{
    public UserMeditation(Guid userId,int meditationId, DateTime time)
    {
        UserId = userId;
        MeditationId = meditationId;
        Time = time;
    }
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public int MeditationId { get; set; }
    public Meditation Meditation { get; set; }
    public DateTime Time { get; set; }
}