using System.Security.Authentication;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using FirebaseAdmin.Auth;
using Server.Entities;
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

    public void Update(UpdateMeditationRequest model, Guid id, string token);

    public Meditation[] GetMeditationByPreferences(MeditationPreferences preferences);
    public void UserListened(string token, Guid meditationId);
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
        var userId = context.GetUserId(token);
        var sub = context.Users.AsQueryable().First(x => x.Id == userId).IsSubscribed || token.Equals("test");
        var meditation = context.Meditations.AsQueryable().First(x => x.id == id);
        var subscription = context.MeditationSubscriptions.AsQueryable().FirstOrDefault(x => x.MeditationId == id);
        if (meditation.IsSubscribed && sub)
            throw new ArgumentException("User did not have subscription");
        return new { Meditation = meditation, Subscription = subscription };
    }

    public Meditation[] GetMeditationByPreferences(MeditationPreferences preferences)
    {
        return context.Meditations.AsQueryable().Where(x =>
                (preferences.Time == null || x.Time == preferences.Time) &&
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
            WriteSubscptionImage(photo,meditation.id);
        }

        if (model.MeditationPhoto != null)
        {
            var photo = Convert.FromBase64String(model.MeditationPhoto);
            WriteMeditationImage(photo,meditation.id);
        }
        if (model.Subscription != null) context.MeditationSubscriptions.Add(model.Subscription);
        meditation.UserMeditations = new List<UserMeditation>();
        context.Meditations.Add(meditation);
        context.SaveChangesAsync();
    }

    public void Update(UpdateMeditationRequest model, Guid id, string token)
    {
        FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        var query = context.Meditations.AsQueryable();
        var meditation = query.First(x => x.id == id);
        mapper.Map(model, meditation);
        context.Meditations.Update(meditation);
        context.SaveChangesAsync();
    }
    
    public void UserListened(string token, Guid meditationId)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        var uid = task.Result.Uid;
        var n = new UserMeditation(uid, meditationId, DateTime.Today);
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

    private void WriteSubscptionImage(byte[] photo,Guid meditationId)
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