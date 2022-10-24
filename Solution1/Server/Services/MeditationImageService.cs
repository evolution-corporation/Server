using Server.Helpers;

namespace Server.Services;

public interface IMeditationImageService
{
    public string GetMeditationImage(Guid id);
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

    public string GetMeditationImage(Guid id) => resources.Storage + resources.MeditationImages +
                                                 context.Meditations.First(x => x.Id == id).PhotoId;
}