using AutoMapper;
using Server.Helpers;

namespace Server.Services;

public interface IUserImageService
{
    public string GetUserImage(Guid id);
}

public class UserImageService: IUserImageService
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

    public string GetUserImage(Guid id)
    {
        var file = File.ReadAllBytes(resources.Images + id);
        var base64 = Convert.ToBase64String(file);
        return base64;
    }
}