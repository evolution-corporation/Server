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
    public string? GetMeditationAudioUrl(Guid meditationId, string userToken);
}

public class MeditationAudioService : IMeditationAudioService
{
    private DataContext context;
    private readonly Resources resources;

    public MeditationAudioService(DataContext context, Resources resources)
    {
        this.context = context;
        this.resources = resources;
    }

    public string? GetMeditationAudioUrl(Guid meditationId, string userId)
    {
        var user = context.Users.AsQueryable().First(x => x.Id == userId);
        var meditation = context.Meditations.AsQueryable().First(x => x.Id == meditationId);
        if (meditation.IsSubscribed)
        {
            if (user.IsSubscribed)
            {
                return resources.Storage + resources.AudioBucket + meditation.Language + meditation.Id;
                // return meditation.AudioUrl;
            }
            throw new AuthenticationException("Попытка получения медитации без подписки");
        }
        return resources.Storage + resources.AudioBucket + meditation.Language + meditation.Id;
    }
}