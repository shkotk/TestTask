using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TestTask.ProjectApi.Interfaces;
using TestTask.ProjectApi.Models;

namespace TestTask.ProjectApi.Services;

public class ProjectStore : IProjectStore
{
    private readonly IMongoCollection<Project> _projectsCollection;

    public ProjectStore(IMongoCollection<Project> projectsCollection)
    {
        _projectsCollection = projectsCollection ?? throw new ArgumentNullException(nameof(projectsCollection));
    }

    public async Task Add(Project project, CancellationToken cancellationToken)
    {
        await _projectsCollection.InsertOneAsync(project, cancellationToken: cancellationToken);
    }

    public async Task<ICollection<Project>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        return await _projectsCollection.AsQueryable().Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }
}