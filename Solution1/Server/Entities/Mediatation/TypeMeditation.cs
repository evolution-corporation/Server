namespace Server.Entities;

public class TypeMeditation
{
    public static TypeMeditation Relaxation = new();
    public static TypeMeditation BreathtakingPractice = new();
    public static TypeMeditation DirectionalVisualizations = new ();
    public static TypeMeditation DancePsychotechnics = new();
    public static TypeMeditation DMD = new();

    private TypeMeditation()
    {
    }

    public static TypeMeditation? Convert(string? str)
    {
        return str switch
        {
            "Relaxation" => Relaxation,
            "BreathtakingPractice" => BreathtakingPractice,
            "DirectionalVisualizations" => DirectionalVisualizations,
            "DancePsychotechnics" => DancePsychotechnics,
            "DMD" => DMD,
            null => null,
            _ => throw new NotImplementedException("We don't have this type of meditation")
        };
    }
}