using AutoMapper;
using FirebaseAdmin.Auth;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Meditation;

namespace WebApi.Services;

public interface IMeditationService
{
    public Meditation GetById(int id);
    public IEnumerable<Meditation> GetAllMeditation();
    public IEnumerable<Meditation> GetNotListened(string? token);
    public Meditation GetPopular();

    public void Create(CreateMeditationRequest model, string token);

    public void Update(UpdateMeditationRequest model, int id, string token);
}

public class MeditationService: IMeditationService
{
    private readonly DataContext context;
    private readonly IMapper mapper;
    public MeditationService(DataContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public Meditation GetById(int id)
    {
        return context.Meditations.First(x => x.id == id);
    }

    public IEnumerable<Meditation> GetAllMeditation()
    {
        return context.Meditations;
    }

    public IEnumerable<Meditation> GetNotListened(string? token)
    {
        var id = new Guid(FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid);
        var user = context.Users.First(x => x.Id == id);
        return context.Meditations.Where(x => !user.ListenedMeditation.Contains(x.id));
    }

    public Meditation GetPopular()
    {
        var max = context.Meditations.Max(x => x.ListenedToday);
        return context.Meditations.First(x => x.ListenedToday == max);
    }

    public void Create(CreateMeditationRequest model, string token)
    {
        FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        var meditation = mapper.Map<Meditation>(model);
        context.Meditations.Add(meditation);
        context.SaveChangesAsync();
    }

    public void Update(UpdateMeditationRequest model,int id, string token)
    {
        FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        var meditation = context.Meditations.First(x => x.id == id);
        mapper.Map(model, meditation);
        context.Meditations.Update(meditation);
        context.SaveChangesAsync();
    }
}