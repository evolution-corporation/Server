namespace Server.Entities;

public class UserMeditation
{
    public UserMeditation(string userId,int meditationId, DateTime time)
    {
        UserId = userId;
        MeditationId = meditationId;
        Time = time;
    }
    public string UserId { get; set; }
    public User User { get; set; }
    
    public int MeditationId { get; set; }
    public Meditation Meditation { get; set; }
    public DateTime Time { get; set; }
}