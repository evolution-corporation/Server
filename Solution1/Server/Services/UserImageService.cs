using AutoMapper;
using FirebaseAdmin.Auth;
using Server.Helpers;

namespace Server.Services;

public interface IUserImageService
{
    public string GetUserImage(string token);
}

public class UserImageService : IUserImageService
{
    private DataContext context;
    private readonly IMapper mapper;
    private readonly Resources resources;

    public UserImageService(DataContext context, IMapper mapper, Resources resources)
    {
        this.context = context;
        this.mapper = mapper;
        this.resources = resources;
    }

    public string GetUserImage(string token)
    {
        var userId = context.GetUserId(token);
        var user = context.Users.First(x => x.Id == userId);
        if (!user.HasPhoto)
            throw new ArgumentException("User don't have a photo");
        var file = File.ReadAllBytes(resources.UserImage + "/" + userId + ".k");
        var base64 = Convert.ToBase64String(file);
        return base64;
    }
}