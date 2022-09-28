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
    void Update(string id, UpdateUserRequest model);
    User UpdateByUser(string token, UpdateUserRequest model);
    public void UserListened(string token, int meditationId);
}

public class UserService : IUserService
{
    private DataContext _context;
    private readonly IMapper _mapper;
    private readonly Resources resources;
    private readonly AmazonS3Client s3;

    public UserService(
        DataContext context,
        IMapper mapper,
        Resources resources,
        AmazonS3Client s3)
    {
        _context = context;
        _mapper = mapper;
        this.resources = resources;
        this.s3 = s3;
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(string id)
    {
        return getUser(id);
    }

    public User Create(CreateUserRequest model, string token)
    {
        // validate
        if (_context.Users.Any(x => x.NickName == model.NickName))
            throw new AppException($"{model.NickName} already taken. You can try to use another nickname"
                                   + GenerateUserNickname(model.NickName));
        if (model.DisplayName != null && ContentFilter.ContainsAbsentWord(model.DisplayName.Split()) ||
            model.Status != null && ContentFilter.ContainsAbsentWord(model.Status.Split()) ||
            model.DisplayName != null && ContentFilter.ContainsAbsentWord(model.DisplayName.Split()))
            throw new AppException("Here is bad word");
        var user = _mapper.Map<User>(model);

        if (token.Equals("test"))
            user.Id = new Random().Next().ToString();
        else
        {
            var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            task.Wait();
            user.Id = task.Result.Uid;
        }

        if (model.Photo != null)
        {
            var photo = Convert.FromBase64String(model.Photo);
            WriteObject(photo, user.Id);
            user.HasPhoto = true;
        }

        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public void Update(string id, UpdateUserRequest model)
    {
        var user = getUser(id);

        // copy model to user and save
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public User UpdateByUser(string token, UpdateUserRequest model)
    {
        var query = _context.Users.AsQueryable();
        var uid = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid;
        var user = query.First(x => x.Id == uid);
        if (user == null)
            throw new AppException("User with token" + token + "not found");
        if (model.NickName != null)
            if (_context.Users.Any(x => x.NickName == model.NickName))
                throw new AppException($"{model.NickName} already taken. You can try to use another nickname"
                                       + string.Join(",", GenerateUserNickname(model.NickName)));
        if (model.Image != null)
        {
            if (user.HasPhoto) DeleteObject(user.Id);;
            var photo = Convert.FromBase64String(model.Image);
            WriteObject(photo,user.Id);
            user.HasPhoto = true;
        }
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }

    public void UserListened(string token, int meditationId)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        var uid = task.Result.Uid;
        var n = new UserMeditation(uid, meditationId, DateTime.Today);
        if (!_context.UserMeditations.Contains(n))
            _context.UserMeditations.Add(n);
        _context.SaveChanges();
    }

    private User getUser(string id)
    {
        var query = _context.Users.AsQueryable();
        var user = query.FirstOrDefault(x => x.Id.Equals(id));
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    private string[] GenerateUserNickname(string nickname)
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