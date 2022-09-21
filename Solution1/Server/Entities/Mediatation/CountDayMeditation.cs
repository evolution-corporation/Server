namespace Server.Entities;

public enum CountDayMeditation
{
    Days2_3,
    Days4_5,
    Days6_7
}

public static class CountDayMeditationConverter
{
    public static CountDayMeditation? Convert(string? str)
    {
        return str switch
        {
            "Days2_3" => CountDayMeditation.Days2_3,
            "Days4_5" => CountDayMeditation.Days4_5,
            "Days6_7" => CountDayMeditation.Days6_7,
            null => null,
            _ => throw new NotImplementedException("We don't have this Count day meditation")
        };
    }
}