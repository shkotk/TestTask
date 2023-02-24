using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.ProjectApi.Extensions;
using TestTask.ProjectApi.Interfaces;
using TestTask.ProjectApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddHttpClient<IUserApiClient, UserApiClient>(options =>
        options.BaseAddress = builder.Configuration.GetValue<Uri>("UserApiHost"));

builder.Services.AddMongoCollections(builder.Configuration)
    .AddScoped<IProjectStore, ProjectStore>()
    .AddScoped<IPopularIndicatorService, PopularIndicatorsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();