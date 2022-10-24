namespace Server.Entities.Mediatation;

public enum TimeMeditation
{
    LessThan15Minutes,
    MoreThan15AndLessThan60Minutes,
    MoreThan60Minutes
}

public static class TimeMeditationConverter
{
    public static TimeMeditation? Convert(string? str)
    {
        return str switch
        {
            "lessThan15Minutes" => TimeMeditation.LessThan15Minutes,
            "moreThan15AndLessThan60Minutes" => TimeMeditation.MoreThan15AndLessThan60Minutes,
            "moreThan60Minutes" => TimeMeditation.MoreThan60Minutes,
            null => null,
            _ => throw new NotImplementedException("We don't have this Time of meditation")
        };
    }
} 