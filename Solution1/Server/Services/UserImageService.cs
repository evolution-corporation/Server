using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using FirebaseAdmin.Auth;
using Server.Entities;
using Server.Helpers;

namespace Server.Services;

public interface IUserImageService
{
    public string GetUserImage(string token);
    public void DeleteUserImage(string token);
}

public class UserImageService : IUserImageService
{
    private DataContext context;
    private readonly IMapper mapper;
    private readonly Resources resources;
    private readonly IAmazonS3 s3;

    public UserImageService(DataContext context, IMapper mapper, Resources resources, IAmazonS3 s3)
    {
        this.context = context;
        this.mapper = mapper;
        this.resources = resources;
        this.s3 = s3;
    }

    public string GetUserImage(string userId)
    {
        var user = context.Users.First(x => x.Id == userId);
        if (user.PhotoId == null)
            throw new ArgumentException("User don't have a photo");
        return resources.Storage + "/" + resources.ImageBucket + "/" + resources.UserImage + user.PhotoId;
    }

    public void DeleteUserImage(string token)
    {
        var user = context.GetUser(token);
        if (user == null)
            throw new KeyNotFoundException();
        DeleteObject(user);
        user.PhotoId = null;
        context.SaveChanges();
    }
    
    private void DeleteObject(User user)
    {
    //     var req = new DeleteObjectRequest
    //     {
    //         BucketName = resources.ImageBucket,
    //         Key = resources.UserImage + user.PhotoId
    //     };
    //     Task<DeleteObjectResponse> res = s3.DeleteObjectAsync(req);
    //     res.Wait();
        s3.DeleteObjectAsync(resources.ImageBucket, resources.UserImage + user.PhotoId);
    }
}