using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface ISubscribeService
{
   public void SubscribeUser(Guid userId, DateTime timeSubscribe);

   public void UnsubscribeUser(Guid userId);
}

public class SubscribeService: ISubscribeService
{
   private readonly DataContext context;

   public SubscribeService(DataContext context)
   {
      this.context = context;
   }

   public void SubscribeUser(Guid userId, DateTime timeSubscribe)
   {
      var sub = new Subscribe
      {
         UserId = userId,
         WhenSubscribe = DateTime.Now,
         TimeSubscribe = timeSubscribe
      };
      context.Users.First(x => x.Id == userId).IsSubscribed = true;
      context.Subscribes.Add(sub);
   }

   public void UnsubscribeUser(Guid userId)
   {
      context.Subscribes.Remove(context.Subscribes.First(x => x.UserId == userId));
      context.Users.First(x => x.Id == userId).IsSubscribed = false;
   }
}