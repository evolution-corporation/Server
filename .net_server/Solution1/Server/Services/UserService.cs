using FirebaseAdmin.Auth;

namespace Server.Services;

using AutoMapper;
using Entities;
using Helpers;
using Models.Users;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User GetById(Guid id);
    void Create(CreateUserRequest model, string token);
    void Update(Guid id, UpdateUserRequest model);
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

    public User GetById(Guid id)
    {
        return getUser(id);
    }

    public void Create(CreateUserRequest model, string token)
    {
        // validate
        if (_context.Users.Any(x => x.NickName == model.NickName))
            throw new AppException($"{model.NickName} already taken. You can try to use another nickname"
                                   + GenerateUserNickname(model.NickName));
        if (ContentFilter.ContainsAbsentWord(model.DisplayName.Split()) ||
            ContentFilter.ContainsAbsentWord(model.Status.Split()) ||
            ContentFilter.ContainsAbsentWord(model.DisplayName.Split()))
            throw new AppException("Here is bad word");
        var user = _mapper.Map<User>(model);
        var base64 = Convert.FromBase64String(model.Image);
        File.WriteAllBytes(resources.UserImage + "/" + user.Id, base64);
        if (!token.Equals("test"))
            user.Id = new Guid(FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid);
        user.ListenedMeditation = new List<int>();
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(Guid id, UpdateUserRequest model)
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
        var uid = new Guid(FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid);
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
        var userId = _context.GetUserId(token);
        var user = _context.Users.AsQueryable().First(x => x.Id == userId);
        var listenedBefore = user.ListenedMeditation.Count;
        if (!user.ListenedMeditation.AsQueryable().Contains(meditationId)) user.ListenedMeditation.Add(meditationId);
        if (user.ListenedMeditation.Count != listenedBefore)
            _context.Meditations.Find(meditationId)!.ListenedToday.Add(userId);
        _context.SaveChanges();
    }

    private User getUser(Guid id)
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