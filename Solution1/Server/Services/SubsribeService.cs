﻿using System.Security.Authentication;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface ISubscribeService
{
   public void SubscribeUser(string token, int timeSubscribe);

   public void UnsubscribeUser(string token);
   public Subscribe GetUserSubscribe(string token);
}

public class SubscribeService: ISubscribeService
{
   private readonly DataContext context;

   public SubscribeService(DataContext context)
   {
      this.context = context;
   }
   //TODO: перенести подписку пользователя с клиента на сервер.
   public void SubscribeUser(string token, int timeSubscribe)
   {
      var userId = context.GetUserId(token);
      if (context.Payments.FirstOrDefault(x => x.UserId == userId) == null)
         throw new AuthenticationException("Subscription not paid");

      var oldSub = context.Subscribes.First(x => x.UserId == userId);
      // var sub = new Subscribe
      // {
      //    UserId = userId,
      //    WhenSubscribe = DateTime.Now,
      //    RemainingTime = timeSubscribe + oldSub.RemainingTime
      // };
      context.Subscribes.Remove(oldSub);
      //context.Subscribes.Add(sub);
      context.SaveChanges();
   }

   public Subscribe GetUserSubscribe(string id)
   {
      return context.Subscribes.AsQueryable().First(x => x.UserId == id);
   }

   public void UnsubscribeUser(string token)
   {
      var userId = context.GetUserId(token);
      context.Subscribes.Remove(context.Subscribes.AsQueryable().First(x => x.UserId == userId));
      context.SaveChanges();
   }
}