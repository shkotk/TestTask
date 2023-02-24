using System.Collections.Generic;

namespace TestTask.UserApi.ApiModels;

public class GetSubscriptionTypesRequest
{
    public ICollection<int> UserIds { get; set; }
}