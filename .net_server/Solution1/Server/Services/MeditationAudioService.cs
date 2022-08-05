using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using NAudio.Wave;
using Server.Helpers;

namespace Server.Services;

public interface IMeditationAudioService
{
    //public void GetAudio(string token, int id, IPEndPoint ip);
    public void GetAudio(ChannelWriter<byte> channel);
}

public class MeditationAudioService : IMeditationAudioService
{

    private DataContext context;
    private Resources resources;

    public MeditationAudioService(DataContext context, Resources resources)
    {
        this.context = context;
        this.resources = resources;
    }

    public void GetAudio(ChannelWriter<byte> channel)
    {
        var stream = new BufferedStream(File.OpenRead("osmin.pdf"));
        new SenderStream().Write(stream, channel);
        //FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

        //var file = Convert.ToBase64String(bytes);
        //return file;
    }
} 