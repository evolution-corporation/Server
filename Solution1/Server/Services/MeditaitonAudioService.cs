using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;

//TODO: Проверить работоспособность

public interface IMeditationAudioService
{
    public string? GetMeditationAudioUrl(int meditationId, string userToken);
}

public class MeditationAudioService: IMeditationAudioService
{
    private DataContext context;
    public MeditationAudioService(DataContext context)
    {
        this.context = context;
    }

    public string? GetMeditationAudioUrl(int meditationId, string userId)
    {
        var user = context.Users.AsQueryable().First(x => x.Id == userId);
        var meditation = context.Meditations.AsQueryable().First(x => x.id == meditationId);
        if(meditation.IsSubscribed)
        {
            if(user.IsSubscribed)
            {
                return meditation.AudioUrl;
            }
            throw new AuthenticationException("Попытка получения медитации без подписки");
        }
        return meditation.AudioUrl;
    }
}