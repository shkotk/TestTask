using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTask.UserApi.Interfaces;
using TestTask.UserApi.Models;

namespace TestTask.UserApi.Services;

public class SubscriptionStore : ISubscriptionStore
{
    private readonly UserDbContext _dbContext;

    public SubscriptionStore(UserDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _dbContext.Subscriptions.Add(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Subscription> GetAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Subscriptions.Where(s => s.Id == id).FirstOrDefaultAsync(cancellationToken);
    }
}