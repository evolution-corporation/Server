namespace Server.Entities;

public class CountDayMeditation
{
    public static CountDayMeditation Days2_3 = new();
    public static CountDayMeditation Days4_5 = new();
    public static CountDayMeditation Days6_7 = new();

    public static CountDayMeditation? Convert(string? str)
    {
        return str switch
        {
            "Days2_3" => Days2_3,
            "Days4_5" => Days4_5,
            "Days6_7" => Days6_7,
            null => null,
            _ => throw new NotImplementedException("We don't have this Count day meditation")
        };
    }
}