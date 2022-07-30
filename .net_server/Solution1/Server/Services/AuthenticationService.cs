using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface IAuthenticationService
{
    public User GetUserByToken(string token);
}

public class AuthenticationService: IAuthenticationService
{
    private readonly DataContext context;
    public AuthenticationService(DataContext context) => this.context = context;

    public User GetUserByToken(string token)
    {
        var decodedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result;
        var uid = new Guid(decodedToken.Uid);
        return context.Users.First(x => x.Id == uid);
    }
}