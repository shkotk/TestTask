using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTask.UserApi.Interfaces;
using TestTask.UserApi.Models;

namespace TestTask.UserApi.Services;

public class UserStore : IUserStore
{
    private readonly UserDbContext _dbContext;

    public UserStore(UserDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SetSubscriptionId(int userId, int subscriptionId, CancellationToken cancellationToken)
    {
        var user = new User {Id = userId};
        _dbContext.Users.Attach(user);
        user.SubscriptionId = subscriptionId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<string>> GetSubscriptionTypes(ICollection<int> userIds, CancellationToken cancellationToken)
    {
        // left join with order by user ID provides subscription types ordered same way as user IDs (if they are ordered and unique)
        var query = from u in _dbContext.Users
            join s in _dbContext.Subscriptions
                on u.SubscriptionId equals s.Id into grouping
            from s in grouping.DefaultIfEmpty()
            where userIds.Contains(u.Id)
            orderby u.Id
            select s.Type;

        return await query.ToArrayAsync(cancellationToken);
    }
}