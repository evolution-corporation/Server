namespace Server.Entities.Mediatation;

public enum TypeMeditation
{
    Relaxation ,
    BreathtakingPractice,
    DirectionalVisualizations,
    Basic,
    Set
}

public static class TypeMeditationConverter {
    public static TypeMeditation? Convert(string? str)
    {
        return str switch
        {
            "relaxation" => TypeMeditation.Relaxation,
            "breathtakingPractice" => TypeMeditation.BreathtakingPractice,
            "directionalVisualizations" => TypeMeditation.DirectionalVisualizations,
            "basic" => TypeMeditation.Basic,
            "set" => TypeMeditation.Set,
            null => null,
            _ => throw new NotImplementedException("We don't have this type of meditation")
        };
    }
}