using Server.Helpers;

namespace Server.Services;

public interface IMeditationImageService
{
    public string GetMeditationImage(int id);
}

public class MeditationImageService : IMeditationImageService
{
    private DataContext context;
    private Resources resources;

    public MeditationImageService(DataContext context, Resources resources)
    {
        this.context = context;
        this.resources = resources;
    }

    public string GetMeditationImage(int id)
    {
        var file = File.ReadAllBytes(resources.MeditationImages + "/" + id + ".k");
        var str = Convert.ToBase64String(file);
        return str;
    }
}