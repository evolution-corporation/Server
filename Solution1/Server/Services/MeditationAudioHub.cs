using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;

//TODO: Проверить работоспособность.
public class AsyncEnumerableHub : Hub
{
    private Resources resources;
    public AsyncEnumerableHub(Resources resources)
    {
        this.resources = resources;
    }

    public ChannelReader<byte> GetAudio(
        int delay,
        CancellationToken cancellationToken,
        int meditationId)
    {
        var channel = Channel.CreateUnbounded<byte>();
        var stream = new BufferedStream(File.OpenRead($"{resources.MeditationAudio}/{meditationId}.mp3"));
        // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
        // for all the items to be written before returning the channel back to
        // the client.
        _ = WriteItemAsync(channel.Writer, delay, cancellationToken, stream);

        return channel.Reader;
    }
    
    public async Task PostAudio(IAsyncEnumerable<byte> stream, int meditationId)
    {
        var file = new BufferedStream(
            new FileStream($"{resources.MeditationAudio}/{meditationId}.mp3", FileMode.Create));
        await foreach (var item in stream) file.WriteByte(item);
    }

    private async Task WriteItemAsync(
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