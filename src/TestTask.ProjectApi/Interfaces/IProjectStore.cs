using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestTask.ProjectApi.Models;

namespace TestTask.ProjectApi.Interfaces;

public interface IProjectStore
{
    Task Add(Project project, CancellationToken cancellationToken);
    Task<ICollection<Project>> GetByUserId(int userId, CancellationToken cancellationToken);
}