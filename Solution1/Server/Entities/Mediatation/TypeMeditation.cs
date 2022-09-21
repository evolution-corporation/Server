namespace Server.Entities;

public enum TypeMeditation
{
    Relaxation ,
    BreathtakingPractice,
    DirectionalVisualizations,
    DancePsychotechnics,
    DMD
}

public static class TypeMeditationConverter {
    public static TypeMeditation? Convert(string? str)
    {
        return str switch
        {
            "Relaxation" => TypeMeditation.Relaxation,
            "BreathtakingPractice" => TypeMeditation.BreathtakingPractice,
            "DirectionalVisualizations" => TypeMeditation.DirectionalVisualizations,
            "DancePsychotechnics" => TypeMeditation.DancePsychotechnics,
            "DMD" => TypeMeditation.DMD,
            null => null,
            _ => throw new NotImplementedException("We don't have this type of meditation")
        };
    }
}