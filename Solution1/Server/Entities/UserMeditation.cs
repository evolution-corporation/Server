using Server.Entities.Mediatation;

namespace Server.Entities;

public class UserMeditation
{
#pragma warning disable CS8618
    public UserMeditation(string user, Guid meditation, DateTime time, string language)
#pragma warning restore CS8618
    {
        User = user;
        Meditation = meditation;
        Time = time;
        Language = language;
    }

    public string User { get; set; }
    //public User User { get; set; }
    
    public Guid Meditation { get; set; }
    public string Language { get; set; }
    //public Meditation Meditation { get; set; }
    public DateTime Time { get; set; }
}