using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.UserApi;
using TestTask.UserApi.Interfaces;
using TestTask.UserApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddDbContextPool<UserDbContext>(contextBuilder => contextBuilder.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"), options => options.EnableRetryOnFailure()))
    .AddAutoMapper(typeof(MappingProfile))
    .AddScoped<IUserStore, UserStore>()
    .AddScoped<ISubscriptionStore, SubscriptionStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    dbContext.Database.EnsureCreated(); // TODO use migrations
}

app.Run();

namespace TestTask.UserApi
{
    public partial class Program { }
}
