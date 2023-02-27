using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Npgsql;

namespace TestTask.EndToEndTests.Helpers;

public class Fixture : IDisposable
{
    public Fixture()
    {
        // Arrange PG connection
        var pgConnectionStringBuilder = new NpgsqlConnectionStringBuilder(
            ConfigManager.Configuration.GetConnectionString("PostgresConnection"))
        {
            Database = "user-db-" + Guid.NewGuid(),
        };
        var pgConnectionString = pgConnectionStringBuilder.ConnectionString;
        _contextOptions = new DbContextOptionsBuilder<UserApi.UserDbContext>()
            .UseNpgsql(pgConnectionString)
            .Options;
        
        // Arrange UserApi
        UserApiHttpClient = new WebApplicationFactory<UserApi.Program>()
            .WithWebHostBuilder(builder =>
            {
                var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:PostgresConnection"] = pgConnectionString,
                }).Build();
                builder.UseConfiguration(config);
            })
            .CreateClient();
        
        // Arrange Mongo connection
        _mongoConnectionString = ConfigManager.Configuration.GetConnectionString("MongoConnection");
        _mongoDbName = "project-db-" + Guid.NewGuid();
        
        // Arrange ProjectApi
        var userApiClient = new ProjectApi.Services.UserApiClient(UserApiHttpClient);
        ProjectApiHttpClient = new WebApplicationFactory<ProjectApi.Program>().WithWebHostBuilder(builder =>
            {
                var config = new ConfigurationBuilder().AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["MongoDb:ConnectionUri"] = _mongoConnectionString,
                        ["MongoDb:DatabaseName"] = _mongoDbName,
                    }).Build();
                builder.UseConfiguration(config);
                builder.ConfigureTestServices(services => services.Replace(
                    new ServiceDescriptor(typeof(ProjectApi.Interfaces.IUserApiClient), userApiClient)));
            })
            .CreateClient();
    }

    public UserApi.UserDbContext GetDbContext() => new(_contextOptions);
    public IMongoDatabase GetMongoDb() => new MongoClient(_mongoConnectionString).GetDatabase(_mongoDbName);
    public HttpClient UserApiHttpClient { get; }
    public HttpClient ProjectApiHttpClient { get; }

    private readonly DbContextOptions<UserApi.UserDbContext> _contextOptions;
    private readonly string _mongoConnectionString;
    private readonly string _mongoDbName;

    public void Dispose()
    {
        using var dbContext = GetDbContext();
        dbContext.Database.EnsureDeleted();

        var mongoClient = new MongoClient(_mongoConnectionString);
        mongoClient.DropDatabase(_mongoDbName);
    }
}