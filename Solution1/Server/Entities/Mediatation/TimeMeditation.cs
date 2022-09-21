namespace Server.Entities;

public class TimeMeditation
{
    public static TimeMeditation LessThan15Minutes, MoreThan15AndLessThan60Minutes, MoreThan60Minutes;

    public static TimeMeditation? Convert(string? str)
    {
        return str switch
        {
            "LessThan15Minutes" => LessThan15Minutes,
            "MoreThan15AndLessThan60Minutes" => MoreThan15AndLessThan60Minutes,
            "MoreThan60Minutes" => MoreThan60Minutes,
            null => null,
            _ => throw new NotImplementedException("We don't have this Time of meditation")
        };
    }
}