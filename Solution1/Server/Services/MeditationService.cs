using System.Security.Authentication;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Entities.Mediatation;
using Server.Helpers;
using Server.Models.Meditation;

namespace Server.Services;

public interface IMeditationService
{
    public object GetById(Guid id, string token);
    public object GetAllMeditation(string language, int countOfMeditations);
    public IEnumerable<Meditation> GetNotListened(string token, string language);
    public Meditation GetPopular(string language);

    public void Create(CreateMeditationRequest model, string token);

    //public void Update(UpdateMeditationRequest model, Guid id, string token);

    public Meditation[] GetMeditationByPreferences(MeditationPreferences preferences);
    public void UserListened(string token, Guid meditationId, string meditationLanguage);
}

public class MeditationService : IMeditationService
{
    private readonly DataContext context;
    private readonly IMapper mapper;
    private readonly Resources resources;
    private readonly AmazonS3Client s3;

    public MeditationService(DataContext context, IMapper mapper, Resources resources, AmazonS3Client s3)
    {
        this.context = context;
        this.mapper = mapper;
        this.resources = resources;
        this.s3 = s3;
    }

    public object GetById(Guid id, string token)
    {
        var user = context.GetUser(token);
        if (user == null)
            throw new NotSupportedException();
        var sub = user.IsSubscribed;
        var meditation = context.Meditations.AsQueryable().First(x => x.Id == id);
        var subscription = context.MeditationSubscriptions.AsQueryable().FirstOrDefault(x => x.Id == id);
        if (meditation.IsSubscribed && sub)
            throw new ArgumentException("User did not have subscription");
        var obj = new
        {
            MeditatiodId = subscription, 
            Title = subscription.Headers,
            subscription.Description,
            Body = subscription.PayloadText.Split("~")
        };
        return new { Meditation = meditation, Subscription = obj };
    }

    public Meditation[] GetMeditationByPreferences(MeditationPreferences preferences)
    {
        return context.Meditations.AsQueryable().Where(x =>
                (preferences.TypeMeditation == null || preferences.TypeMeditation == x.TypeMeditation))
            .ToArray();
    }


    public object GetAllMeditation(string language, int countOfMeditations)
    {
        var meditations = context.Meditations.AsQueryable().Where(x => x.Language == language).ToArray();
        return new
        {
            meditations, meditations.Length
        };
    }

    public IEnumerable<Meditation> GetNotListened(string token, string language)
    {
        var queryUser = context.Users.AsQueryable();
        var queryMeditation = context.Meditations.AsQueryable();
        var id = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid;
        //var user = queryUser.First(x => x.Id == id).UserMeditations.Select(x => x.MeditationId);
        var user = context.UserMeditations
            .Where(x => x.User.Equals(id))
            .Select(x => x.Meditation);
        return queryMeditation
            .Where(x => x.Language == language && x.TypeMeditation != TypeMeditation.Set)
            .Select(x => x.Id)
            .Except(user)
            .Select(x => queryMeditation.First(y => y.Id == x));
    }

    public Meditation GetPopular(string language)
    {
        var query = context.Meditations.AsQueryable();
        var max = query
            .Where(x => x.Language == language && x.TypeMeditation != TypeMeditation.Set)
            .Max(x => x.UserMeditations.Count);
        return query.First(x =>
            x.UserMeditations.Count == max && x.Language == language && x.TypeMeditation != TypeMeditation.Set);
    }

    public void Create(CreateMeditationRequest model, string token)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        var userId = task.Result.Uid;
        var user = context.Users.First(x => x.Id == userId);
        if (user.Role != Role.ADMIN)
            throw new AuthenticationException("You don't have permision");
        var meditation = mapper.Map<Meditation>(model);
        if (model.SubscriptionPhoto != null)
        {
            var photo = Convert.FromBase64String(model.SubscriptionPhoto);
            WriteSubscptionImage(photo, meditation.Id);
        }

        if (model.MeditationPhoto != null)
        {
            var photo = Convert.FromBase64String(model.MeditationPhoto);
            WriteMeditationImage(photo, meditation.Id);
        }

        if (model.Subscription != null) context.MeditationSubscriptions.Add(model.Subscription);
        meditation.UserMeditations = new List<UserMeditation>();
        context.Meditations.Add(meditation);
        context.SaveChangesAsync().Wait();
    }

    // public void Update(UpdateMeditationRequest model, Guid id, string token)
    // {
    //     FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
    //     var query = context.Meditations.AsQueryable();
    //     var meditation = query.First(x => x.Id == id);
    //     mapper.Map(model, meditation);
    //     context.Meditations.Update(meditation);
    //     context.SaveChangesAsync();
    // }
    //
    public void UserListened(string token, Guid meditationId, string meditationLanguage)
    {
        var user = context.GetUser(token);
        var n = new UserMeditation(user.Id, meditationId, DateTime.Today, meditationLanguage);
        if (!context.UserMeditations.Contains(n))
            context.UserMeditations.Add(n);
        context.SaveChanges();
    }

    //TODO: Переделать на запись в папку по языку
    private void WriteMeditationImage(byte[] photo, Guid meditationId)
    {
        var ms = new MemoryStream(photo);
        var req = new PutObjectRequest
        {
            BucketName = resources.ImageBucket,
            Key = resources.MeditationImages + meditationId,
            InputStream = ms
        };
        var task = s3.PutObjectAsync(req);
        task.Wait();
    }

    private void WriteSubscptionImage(byte[] photo, Guid meditationId)
    {
        var ms = new MemoryStream(photo);
        var req = new PutObjectRequest
        {
            BucketName = resources.ImageBucket,
            Key = resources.SubscriptionImages + meditationId,
            InputStream = ms
        };
        var task = s3.PutObjectAsync(req);
        task.Wait();
    }
}