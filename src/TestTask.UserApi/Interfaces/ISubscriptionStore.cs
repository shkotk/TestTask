using System.Threading;
using System.Threading.Tasks;
using TestTask.UserApi.Models;

namespace TestTask.UserApi.Interfaces;

public interface ISubscriptionStore
{
    Task AddAsync(Subscription user, CancellationToken cancellationToken);
    Task<Subscription> GetAsync(int id, CancellationToken cancellationToken);
}