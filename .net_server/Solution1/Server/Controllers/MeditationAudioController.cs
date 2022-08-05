using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Services;

namespace Server.Controllers;
//
// [ApiController]
// [Route("/meditation.audio")]
// public class MeditationAudioController : ControllerBase
// {
//     private IMeditationAudioService service;
//
//     public MeditationAudioController(IMeditationAudioService service)
//     {
//         this.service = service;
//     }
//
//     // [HttpGet("{token}")]
//     // public IActionResult GetMeditationAudio(string token, int id)
//     // {
//     //     var port = HttpContext.Connection.RemotePort;
//     //     var ip = new IPEndPoint(HttpContext.Connection.RemoteIpAddress ?? throw new InvalidOperationException(), port);
//     //     service.GetAudio(token, id, ip);
//     //     return Ok();
//     // }
//
//     [HttpGet]
//     public IActionResult Get(ChannelWriter<byte> channel)
//     {
//         service.GetAudio(channel);
//
//         return Ok();
//     }
// }

public class AsyncEnumerableHub : Hub
{
    public ChannelReader<byte> Counter(
        int delay,
        CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<byte>();
        var stream = new BufferedStream(File.OpenRead("osmin.pdf"));
        // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
        // for all the items to be written before returning the channel back to
        // the client.
        _ = WriteItemsAsync(channel.Writer, delay, cancellationToken, stream);

        return channel.Reader;
    }

    private async Task WriteItemsAsync(
        ChannelWriter<byte> writer,
        int delay,
        CancellationToken cancellationToken,
        BufferedStream stream)
    {
        Exception localException = null;
        try
        {
            while (true)
            {
                var x = stream.ReadByte();
                if (x == -1)
                    break;
                await writer.WriteAsync(Convert.ToByte(x), cancellationToken);

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(delay, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            localException = ex;
        }
        finally
        {
            writer.Complete(localException);
        }
    }
}