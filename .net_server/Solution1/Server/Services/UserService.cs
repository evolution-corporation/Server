using FirebaseAdmin.Auth;

namespace WebApi.Services;

using AutoMapper;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User GetById(Guid id);
    void Create(CreateUserRequest model, string token);
    void Update(Guid id, UpdateUserRequest model);

    void UpdateByUser(string token, UpdateUserRequest model);
    // void Delete(int id);
}

public class UserService : IUserService
{
    private DataContext _context;
    private readonly IMapper _mapper;

    public UserService(
        DataContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
            throw new AppException("User with the nickname '" + model.NickName + "' already exists");
        // map model to new user object
        var user = _mapper.Map<User>(model);
        user.Id = new Guid(FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid);
        
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
        var uid = new Guid(FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result.Uid);
        var user = _context.Users.FirstOrDefault(x => x.Id == uid);
        if (user == null)
            throw new AppException("User with token" + token + "not found");
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }
    

    // helper methods

    private User getUser(Guid id)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}