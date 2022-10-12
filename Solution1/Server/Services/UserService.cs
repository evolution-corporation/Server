using System.Security.Authentication;
using Amazon.S3;
using Amazon.S3.Model;
using FirebaseAdmin.Auth;

namespace Server.Services;

using AutoMapper;
using Entities;
using Helpers;
using Models.Users;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User GetById(string id);
    User Create(CreateUserRequest model, string token);
    void Update(string adminId, UpdateUserRequest model, string userId);
    User UpdateByUser(string token, UpdateUserRequest model);
}

public class UserService : IUserService
{
    private readonly DataContext context;
    private readonly IMapper mapper;
    private readonly Resources resources;
    private readonly AmazonS3Client s3;

    public UserService(
        DataContext context,
        IMapper mapper,
        Resources resources,
        AmazonS3Client s3)
    {
        this.context = context;
        this.mapper = mapper;
        this.resources = resources;
        this.s3 = s3;
    }

    public IEnumerable<User> GetAll()
    {
        return context.Users;
    }

    public User GetById(string id)
    {
        return GetUser(id);
    }

    public User Create(CreateUserRequest model, string token)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        var Id = task.Result.Uid;

        // validate
        if (context.Users.Any(x => x.NickName == model.NickName) ||
            NicknameService.bookedNickname.ContainsKey(model.NickName) &&
            NicknameService.bookedNickname[model.NickName].Item1.Equals(Id))
            throw new AppException($"{model.NickName} already taken. You can try to use another nickname"
                                   + GenerateUserNickname(model.NickName));
        if (model.DisplayName != null && ContentFilter.ContainsAbsentWord(model.DisplayName.Split()) ||
            model.DisplayName != null && ContentFilter.ContainsAbsentWord(model.DisplayName.Split()))
            throw new AppException("Here is bad word");
        var user = mapper.Map<User>(model);

        user.Id = Id;

        if (model.Image != null)
        {
            Console.WriteLine(model.Image);
            var photo = Convert.FromBase64String(model.Image);
            WriteObject(photo, user.Id);
            user.HasPhoto = true;
        }

        user.UserMeditations = new List<UserMeditation>();
        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    public void Update(string adminId, UpdateUserRequest model, string userId)
    {
        var admin = GetUser(adminId);
        if (admin.Role != Role.ADMIN)
            throw new AuthenticationException("You don't have permission");
        var user = GetUser(userId);
        // copy model to user and save
        mapper.Map(model, user);
        context.Users.Update(user);
        context.SaveChanges();
    }

    public User UpdateByUser(string token, UpdateUserRequest model)
    {
        var query = context.Users.AsQueryable();
        var uid = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid;
        var user = query.First(x => x.Id == uid);
        if (user == null)
            throw new AppException("User with token" + token + "not found");
        if (model.NickName != null)
            if (context.Users.Any(x => x.NickName == model.NickName))
                throw new AppException($"{model.NickName} already taken. You can try to use another nickname"
                                       + string.Join(",", GenerateUserNickname(model.NickName)));
        if (model.Image != null)
        {
            if (user.HasPhoto) DeleteObject(user.Id);
            var photo = Convert.FromBase64String(model.Image);
            WriteObject(photo, user.Id);
            user.HasPhoto = true;
        }

        mapper.Map(model, user);
        context.Users.Update(user);
        context.SaveChanges();
        return user;
    }

    private User GetUser(string id)
    {
        var query = context.Users.AsQueryable();
        var user = query.FirstOrDefault(x => x.Id.Equals(id));
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    private static string[] GenerateUserNickname(string nickname)
    {
        var list = new List<string>();
        var rnd = new Random();
        for (var i = 0; i < 5; i++) list.Add(nickname + rnd.Next());
        return list.ToArray();
    }

    private void WriteObject(byte[] photo, string userId)
    {
        var ms = new MemoryStream(photo);
        var req = new PutObjectRequest
        {
            BucketName = resources.ImageBucket,
            Key = resources.UserImage + userId,
            InputStream = ms
        };
        var task = s3.PutObjectAsync(req);
        task.Wait();
    }

    private void DeleteObject(string userId)
    {
        var req = new DeleteObjectRequest
        {
            BucketName = resources.ImageBucket,
            Key = resources.UserImage + userId
        };
        var task = s3.DeleteObjectAsync(req);
        task.Wait();
    }
}