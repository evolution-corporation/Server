using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Server.Helpers;

public class SenderStream
{
    public async void Write(BufferedStream stream,ChannelWriter<byte> channel)
    {
        while (await channel.WaitToWriteAsync())
        {
            var x = stream.ReadByte();
            if (x == -1) break;
            var send = Convert.ToByte(x);
            channel.TryWrite(send);
        }
    }
    
    private async Task WriteItemsAsync(
        ChannelWriter<byte> writer,
        int count,
        int delay,
        CancellationToken cancellationToken, BufferedStream stream)
    {
        Exception localException = null;
        try
        {
            for (var i = 0; i < count; i++)
            {
                var x = stream.ReadByte();
                if(x == -1) 
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