using System.Collections.Generic;

namespace TestTask.UserApi.ApiModels;

public class GetSubscriptionTypesResponse
{
    public ICollection<string> SubscriptionTypes { get; set; }
}