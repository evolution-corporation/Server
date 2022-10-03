using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface IAuthenticationService
{
    public User? GetUserByToken(string token);
}

public class AuthenticationService: IAuthenticationService
{
    private readonly DataContext context;
    public AuthenticationService(DataContext context) => this.context = context;

    public User? GetUserByToken(string token)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        var decodedToken = task.Result; 
        var uid = decodedToken.Uid;
        var user = context.Users.AsQueryable().FirstOrDefault(x => x.Id == uid);
        return user;
    }
}