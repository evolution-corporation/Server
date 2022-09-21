namespace Server.Entities;

public enum TypeMeditation
{
    Relaxation ,
    BreathtakingPractice,
    DirectionalVisualizations,
    Basic,
    DMD
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
            "DMD" => TypeMeditation.DMD,
            null => null,
            _ => throw new NotImplementedException("We don't have this type of meditation")
        };
    }
}