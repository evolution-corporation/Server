using AutoMapper;
using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface INicknameService
{
    public User? GetUserByNickname(string nickname);
    public void NicknameBooking(string nickname, string token);
}

public class NicknameService : INicknameService
{
    private readonly DataContext context;
    private readonly IMapper mapper;
    public static readonly Dictionary<string, Tuple<string, DateTime>> bookedNickname;

    static NicknameService()
    {
        bookedNickname = new Dictionary<string, Tuple<string, DateTime>>();
    }

    public static void Run()
    {
        var timeSpan = new TimeSpan(0, 3, 0);
        while (true)
        {
            foreach (var pair in from pair in bookedNickname
                     where DateTime.Now - pair.Value.Item2 < timeSpan
                     select pair)
                bookedNickname.Remove(pair.Key);
        }
    }

    public NicknameService(DataContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public User? GetUserByNickname(string nickname)
    {
        return context.Users.AsQueryable().FirstOrDefault(x => x.NickName.Equals(nickname));
    }

    public void NicknameBooking(string nickname, string token)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        var id = task.Result.Uid;
        bookedNickname.Add(nickname, new Tuple<string, DateTime>(id, DateTime.Now));
    }
}