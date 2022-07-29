using AutoMapper;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services;

public interface INicknameService
{
    public User GetUserByNickname(string nickname);
}

public class NicknameService: INicknameService
{
    private DataContext _context;
    private readonly IMapper _mapper;
    
    public NicknameService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public User GetUserByNickname(string nickname)
    {
        return _context.Users.First(x => x.NickName.Equals(nickname));
    }
}