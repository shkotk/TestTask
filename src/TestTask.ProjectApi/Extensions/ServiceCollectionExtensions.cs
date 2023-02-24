using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TestTask.ProjectApi.Models;

namespace TestTask.ProjectApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoCollections(this IServiceCollection services, IConfiguration config)
    {
        var configSection = config.GetSection("MongoDb");
        var client = new MongoClient(configSection.GetValue<string>("ConnectionUri"));
        var db = client.GetDatabase(configSection.GetValue<string>("DatabaseName"));

        var projectsCollection = db.GetCollection<Project>("projects");
        services.AddSingleton(projectsCollection);

        return services;
    }
}