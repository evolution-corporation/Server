using System.Runtime.ExceptionServices;
using System.Security.Claims;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services;

public interface IAuthenticationService
{
    public User GetUserByToken(string token);
}

public class AuthenticationService: IAuthenticationService
{
    private DataContext context;
    public AuthenticationService(DataContext context)
    {
        this.context = context;
    }

    public User GetUserByToken(string token)
    {
        var decodedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token).Result;
        var uid = new Guid(decodedToken.Uid);
        return context.Users.First(x => x.Id == uid);
    }
}