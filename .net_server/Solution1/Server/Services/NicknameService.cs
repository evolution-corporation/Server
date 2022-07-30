using AutoMapper;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface INicknameService
{
    public User GetUserByNickname(string nickname);
}

public class NicknameService: INicknameService
{
    private readonly DataContext context;
    private readonly IMapper mapper;
    
    public NicknameService(DataContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public User GetUserByNickname(string nickname)
    {
        return context.Users.First(x => x.NickName.Equals(nickname));
    }
}