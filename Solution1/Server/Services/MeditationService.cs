using AutoMapper;
using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;
using Server.Models.Meditation;

namespace Server.Services;

public interface IMeditationService
{
    public object GetById(int id, string token);
    public IEnumerable<Meditation> GetAllMeditation(string language);
    public IEnumerable<Meditation> GetNotListened(string token, string language);
    public Meditation GetPopular(string language);

    public void Create(CreateMeditationRequest model);

    public void Update(UpdateMeditationRequest model, int id, string token);

    public Meditation[] GetMeditationByPreferences(MeditationPreferences preferences);
    public int GetCountOfMeditation(MeditationPreferences? preferences);
}

public class MeditationService : IMeditationService
{
    private readonly DataContext context;
    private readonly IMapper mapper;
    private readonly Resources resources;

    public MeditationService(DataContext context, IMapper mapper, Resources resources)
    {
        this.context = context;
        this.mapper = mapper;
        this.resources = resources;
    }

    public object GetById(int id, string token)
    {
        var userId = context.GetUserId(token);
        var sub = context.Subscribes.AsQueryable().FirstOrDefault(x => x.UserId == userId);
        var meditation = context.Meditations.AsQueryable().First(x => x.id == id);
        var subscription = context.MeditationSubscriptions.AsQueryable().FirstOrDefault(x => x.MeditationId == id);
        if (meditation.IsSubscribed && sub == null)
            throw new ArgumentException("User did not have subscription");
        return new { Meditation = meditation, Subscription = subscription };
    }

    public Meditation[] GetMeditationByPreferences(MeditationPreferences preferences)
    {
        return context.Meditations.AsQueryable().Where(x =>
                (preferences.CountDay == null || x.CountDay == preferences.CountDay) &&
                (preferences.Time == null || x.Time == preferences.Time) &&
                (preferences.TypeMeditation == null || preferences.TypeMeditation == x.TypeMeditation))
            .ToArray();
    }

    public int GetCountOfMeditation(MeditationPreferences? preferences)
    {
        return preferences == null
            ? context.Meditations.AsQueryable().Count()
            : context.Meditations.AsQueryable().Count(x =>
                x.TypeMeditation == preferences.TypeMeditation || preferences.TypeMeditation == null);
        ;
    }

    public IEnumerable<Meditation> GetAllMeditation(string language)
    {
        return context.Meditations.AsQueryable().Where(x => x.Language == language).ToArray();
    }

    public IEnumerable<Meditation> GetNotListened(string token, string language)
    {
        var queryUser = context.Users.AsQueryable();
        var queryMeditation = context.Meditations.AsQueryable();
        var id = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid;
        var user = queryUser.First(x => x.Id == id).UserMeditations.Select(x => x.MeditationId);
        return queryMeditation
            .Where(x => x.Language == language)
            .Select(x => x.id)
            .Except(user)
            .Select(x => queryMeditation.First(y => y.id == x));
    }

    public Meditation GetPopular(string language)
    {
        var query = context.Meditations.AsQueryable();
        var max = query
            .Where(x => x.Language == language)
            .Max(x => x.UserMeditations.Count);
        return query.First(x => x.UserMeditations.Count == max && x.Language == language);
    }

    public void Create(CreateMeditationRequest model)
    {
        var meditation = mapper.Map<Meditation>(model);
        if (model.SubscriptionPhoto != null)
        {
            var photo = Convert.FromBase64String(model.SubscriptionPhoto);
            var file = new FileStream(resources.MeditationSubscribtionImage + "/" + meditation.id, FileMode.Create);
            file.Write(photo, 0, photo.Length);
            file.Close();
        }

        if (model.MeditationPhoto != null)
        {
            var photo = Convert.FromBase64String(model.MeditationPhoto);
            var file = new FileStream(resources.UserImage + "/" + meditation.id, FileMode.Create);
            file.Write(photo, 0, photo.Length);
            file.Close();
        }

        if (model.Subscription != null) context.MeditationSubscriptions.Add(model.Subscription);
        context.Meditations.Add(meditation);
        context.SaveChangesAsync();
    }

    public void Update(UpdateMeditationRequest model, int id, string token)
    {
        FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        var query = context.Meditations.AsQueryable();
        var meditation = query.First(x => x.id == id);
        mapper.Map(model, meditation);
        if (!string.IsNullOrEmpty(model.Audio))
        {
            var base64 = Convert.FromBase64String(model.Audio);
            File.Delete(resources.MeditationAudio + "/" + id + ".k");
            File.WriteAllBytes(resources.MeditationAudio + "/" + id + ".k", base64);
        }

        context.Meditations.Update(meditation);
        context.SaveChangesAsync();
    }
}