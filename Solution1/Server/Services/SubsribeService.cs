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
      var user = context.GetUser(token);
      if (user == null)
         throw new NotSupportedException();
      return context.Subscribes.AsQueryable().First(x => x.UserId == user.Id);
   }

   public Subscribe GetUserSubscribeByAdmin(string userId, string token)
   {
      var admin = context.GetUser(token);
      if (admin == null)
         throw new NotSupportedException();
      if (admin.Role != Role.ADMIN)
      {
         throw new AuthenticationException("You don't have permission");
      }
      return context.Subscribes.AsQueryable().First(x => x.UserId == userId);
   }
}