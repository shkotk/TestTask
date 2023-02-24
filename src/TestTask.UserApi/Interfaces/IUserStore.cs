using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestTask.UserApi.Models;

namespace TestTask.UserApi.Interfaces;

public interface IUserStore
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<User> GetAsync(int id, CancellationToken cancellationToken);
    Task SetSubscriptionId(int userId, int subscriptionId, CancellationToken cancellationToken);
    Task<ICollection<string>> GetSubscriptionTypes(ICollection<int> userIds, CancellationToken cancellationToken);
}