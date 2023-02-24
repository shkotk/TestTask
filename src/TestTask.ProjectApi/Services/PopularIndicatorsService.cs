using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TestTask.ProjectApi.Interfaces;
using TestTask.ProjectApi.Models;

namespace TestTask.ProjectApi.Services;

public class PopularIndicatorsService : IPopularIndicatorService
{
    private readonly IMongoCollection<Project> _projectsCollection;
    private readonly IUserApiClient _userApiClient;

    public PopularIndicatorsService(IMongoCollection<Project> projectsCollection, IUserApiClient userApiClient)
    {
        _projectsCollection = projectsCollection ?? throw new ArgumentNullException(nameof(projectsCollection));
        _userApiClient = userApiClient ?? throw new ArgumentNullException(nameof(userApiClient));
    }

    public async Task<ICollection<PopularIndicator>> Get(string subscriptionType, int top, CancellationToken cancellationToken)
    {
        // I'm pretty sure that it isn't the best query, but I'm not an expert of MongoDB and had no time to read enough docs :(
        using var cursor = await _projectsCollection.AsQueryable(new AggregateOptions {BatchSize = 1000}) // TODO move batch size to config
            .SelectMany(project => project.Charts.Select(chart => new {project.UserId, chart}))
            .SelectMany(x => x.chart.Indicators.Select(indicator => new {x.UserId, IndicatorName = indicator.Name}))
            .ToCursorAsync(cancellationToken);

        var buckets = new Dictionary<string, int>();
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            // sort to get subscription types in the same order (it all seems a bit fragile thought)
            var batch = cursor.Current.OrderBy(b => b.UserId).ToArray();
            var groupedBatch = batch.GroupBy(b => b.UserId, b => b.IndicatorName).ToArray();

            var userIds = groupedBatch.Select(x => x.Key).ToArray();
            var subscriptionTypes = await _userApiClient.GetSubscriptionTypes(userIds, cancellationToken);

            var filteredBatch = groupedBatch.Zip(subscriptionTypes).Where(zip => zip.Second == subscriptionType).Select(zip => zip.First);
            foreach (var grouping in filteredBatch)
            {
                foreach (var indicatorName in grouping)
                {
                    buckets[indicatorName] = buckets.GetValueOrDefault(indicatorName) + 1;
                }
            }
        }

        return buckets.OrderByDescending(pair => pair.Value).Take(top)
            .Select(pair => new PopularIndicator
            {
                Name = pair.Key,
                Used = pair.Value,
            }).ToArray();
    }
}