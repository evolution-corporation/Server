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
    void Create(CreateUserRequest model, string token);
    void Update(string id, UpdateUserRequest model);
    void UpdateByUser(string token, UpdateUserRequest model);
    public void UserListened(string token, int meditationId);
}

public class UserService : IUserService
{
    private DataContext _context;
    private readonly IMapper _mapper;
    private readonly Resources resources;

    public UserService(
        DataContext context,
        IMapper mapper,
        Resources resources)
    {
        _context = context;
        _mapper = mapper;
        this.resources = resources;
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(string id)
    {
        return getUser(id);
    }

    public void Create(CreateUserRequest model, string token)
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
        if (model.Image != null)
        {
            var base64 = Convert.FromBase64String(model.Image);
            File.WriteAllBytes(resources.UserImage + "/" + user.Id, base64);
            user.HasPhoto = true;
        }
        if (!token.Equals("test"))
            user.Id = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid;
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(string id, UpdateUserRequest model)
    {
        var user = getUser(id);

        // copy model to user and save
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void UpdateByUser(string token, UpdateUserRequest model)
    {
        var query = _context.Users.AsQueryable();
        var uid = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid;
        var user = query.First(x => x.Id == uid);
        if (user == null)
            throw new AppException("User with token" + token + "not found");
        if (_context.Users.Any(x => x.NickName == model.NickName))
            throw new AppException($"{model.NickName} already taken. You can try to use another nickname"
                                   + GenerateUserNickname(model.NickName));
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
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
        var user = query.FirstOrDefault(x => x.Id == id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    private IEnumerable<string> GenerateUserNickname(string nickname)
    {
        var list = new List<string>();
        var rnd = new Random();
        for (var i = 0; i < 5; i++) list.Add(nickname + rnd.Next());
        return list.ToArray();
    }
}