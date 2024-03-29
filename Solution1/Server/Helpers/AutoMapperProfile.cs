using Server.Models.Meditation;

namespace Server.Helpers;

using AutoMapper;
using Entities;
using Models.Users;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // CreateRequest -> User
        CreateMap<CreateUserRequest, User>();

        // UpdateRequest -> User
        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore both null & empty string properties
                    if (prop == null) return false;
                    if (prop is string arg3 && string.IsNullOrEmpty(arg3)) return false;

                    // ignore null role
                    if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                    return true;
                }
            ));
        CreateMap<CreateMeditationRequest, Meditation>();
        CreateMap<UpdateMeditationRequest, Meditation>().ForAllMembers(x => x.Condition(
            (src, dest, prop) =>
            {
                // ignore both null & empty string properties
                if (prop == null) return false;
                if (prop is string arg3 && string.IsNullOrEmpty(arg3)) return false;
                
                return true;
            }
        ));
    }
}