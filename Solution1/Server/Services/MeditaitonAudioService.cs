using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;

//TODO: Проверить работоспособность

public interface IMeditationAudioService
{
    public string? GetMeditationAudioUrl(int meditationId);
}

public class MeditationAudioService: IMeditationAudioService
{
    private DataContext context;
    public MeditationAudioService(DataContext context)
    {
        this.context = context;
    }

    public string? GetMeditationAudioUrl(int meditationId)
    {
        return context.Meditations.AsQueryable().First(x => x.id == meditationId).AudioUrl;
    }
}