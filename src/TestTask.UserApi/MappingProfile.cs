using AutoMapper;

namespace TestTask.UserApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApiModels.CreateUserRequest, Models.User>();
        CreateMap<Models.User, ApiModels.CreateUserResponse>();
        CreateMap<Models.User, ApiModels.GetUserResponse>();

        CreateMap<ApiModels.CreateSubscriptionRequest, Models.Subscription>();
        CreateMap<Models.Subscription, ApiModels.CreateSubscriptionResponse>();
        CreateMap<Models.Subscription, ApiModels.GetUserResponse>();
    }
}