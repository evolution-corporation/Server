using System.Diagnostics.CodeAnalysis;
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

    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: Enumerator[System.String,System.Tuple`2[System.String,System.DateTime]]")]
    public static void Run()
    {
        var timeSpan = new TimeSpan(0, 3, 0);
        while (true)
        {
            var list = (from pair in bookedNickname
                where DateTime.Now - pair.Value.Item2 < timeSpan
                select pair).ToList();
            foreach (var pair in list)
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
        //var user = context.GetUser(token);
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        bookedNickname.Add(nickname, new Tuple<string, DateTime>(task.Result.Uid, DateTime.Now));
    }
}