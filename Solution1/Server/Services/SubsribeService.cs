using System.Security.Authentication;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface ISubscribeService
{

   public Subscribe GetUserSubscribe(string token);
   public Subscribe GetUserSubscribeByAdmin(string userId, string token);
}

public class SubscribeService: ISubscribeService
{
   private readonly DataContext context;

   public SubscribeService(DataContext context)
   {
      this.context = context;
   }

   public Subscribe GetUserSubscribe(string token)
   {
      var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
      task.Wait();
      var id = task.Result.Uid;
      return context.Subscribes.AsQueryable().First(x => x.UserId == id);
   }

   public Subscribe GetUserSubscribeByAdmin(string userId, string token)
   {
      var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
      task.Wait();
      var admin = context.Users.First(x => x.Id == task.Result.Uid);
      if (admin.Role != Role.ADMIN)
      {
         throw new AuthenticationException("You don't have permission");
      }
      return context.Subscribes.AsQueryable().First(x => x.UserId == userId);
   }
}