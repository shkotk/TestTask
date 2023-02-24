using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestTask.ProjectApi.Interfaces;

public interface IUserApiClient
{
    Task<ICollection<string>> GetSubscriptionTypes(ICollection<int> userIds, CancellationToken cancellationToken);
}