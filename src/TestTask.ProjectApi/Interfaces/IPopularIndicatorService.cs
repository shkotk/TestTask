using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestTask.ProjectApi.Models;

namespace TestTask.ProjectApi.Interfaces;

public interface IPopularIndicatorService
{
    Task<ICollection<PopularIndicator>> GetAsync(string subscriptionType, int top, CancellationToken cancellationToken);
}