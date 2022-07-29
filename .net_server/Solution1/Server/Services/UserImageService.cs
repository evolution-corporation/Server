using AutoMapper;
using WebApi.Helpers;

namespace WebApi.Services;

public interface IUserImageService
{
    public Guid GetUserImage(Guid id);
}

public class UserImageService: IUserImageService
{
    private DataContext _context;
    private readonly IMapper _mapper;
    
    public UserImageService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Guid GetUserImage(Guid id) => _context.Users.First(x => x.Id == id).ImageId;
}