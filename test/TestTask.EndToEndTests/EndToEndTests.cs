using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTask.EndToEndTests.Helpers;
using Xunit;

namespace TestTask.EndToEndTests;

/// <summary>
/// ⚠️DISCLAIMER⚠️
/// These tests were added after 24 hour deadline because I made a dumb typo in fixture setup
/// and spent a lot of time looking for some routing/WebApplicationFactory issue.
/// </summary>
[TestCaseOrderer("TestTask.EndToEndTests.Helpers.PriorityOrderer", "TestTask.EndToEndTests")]
public class EndToEndTests : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public EndToEndTests(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [TestPriority(0)]
    public async Task UserApi_CreateUser_AddsUsersToDb()
    {
        // Arrange
        var requestBodies = new[]
        {
            @"{""name"":""John Doe"",""email"":""John@example.com""}",
            @"{""name"":""Mark Shimko"",""email"":""Mark@example.com""}",
            @"{""name"":""Taras Ovruch"",""email"":""Taras@example.com""}",
        };
        
        // Act
        var responses = new List<HttpResponseMessage>();
        foreach (var body in requestBodies)
        {
            var response = await _fixture.UserApiHttpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Post, "/user")
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json"),
                });
            responses.Add(response);
        }
        
        // Assert
        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
        
        await using var assertDbContext = _fixture.GetDbContext();
        var users = await assertDbContext.Users.OrderBy(u => u.Id).ToArrayAsync();
        Assert.Equal(3, users.Length);

        Assert.Equal(1, users[0].Id);
        Assert.Equal("John Doe", users[0].Name);
        Assert.Equal("John@example.com", users[0].Email);
        Assert.Null(users[0].SubscriptionId);

        Assert.Equal(2, users[1].Id);
        Assert.Equal("Mark Shimko", users[1].Name);
        Assert.Equal("Mark@example.com", users[1].Email);
        Assert.Null(users[1].SubscriptionId);

        Assert.Equal(3, users[2].Id);
        Assert.Equal("Taras Ovruch", users[2].Name);
        Assert.Equal("Taras@example.com", users[2].Email);
        Assert.Null(users[2].SubscriptionId);
    }

    [Theory]
    [InlineData(1, @"{""id"":1,""name"":""John Doe"",""email"":""John@example.com"",""subscriptionId"":null}")]
    [InlineData(2, @"{""id"":2,""name"":""Mark Shimko"",""email"":""Mark@example.com"",""subscriptionId"":null}")]
    [InlineData(3, @"{""id"":3,""name"":""Taras Ovruch"",""email"":""Taras@example.com"",""subscriptionId"":null}")]
    [TestPriority(1)]
    public async Task UserApi_GetUser_ReturnsPreviouslyAddedUser(int userId, string expectedResponseBody)
    {
        // Act
        var response = await _fixture.UserApiHttpClient.GetAsync($"/user/{userId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponseBody, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    [TestPriority(2)]
    public async Task UserApi_CreateSubscription_AddsSubscriptionsToDb()
    {
        // Arrange
        var requestBodies = new[]
        {
            @"{""type"":""Free"",""startDate"":""2022-05-17T15:28:19Z"",""endDate"":""2099-01-01T00:00:00Z""}",
            @"{""type"":""Super"",""startDate"":""2022-05-18T15:28:19Z"",""endDate"":""2022-08-18T15:28:19Z""}",
            @"{""type"":""Trial"",""startDate"":""2022-05-19T15:28:19Z"",""endDate"":""2022-06-19T15:28:19Z""}",
            @"{""type"":""Free"",""startDate"":""2022-05-20T15:28:19Z"",""endDate"":""2099-01-01T00:00:00Z""}",
            @"{""type"":""Trial"",""startDate"":""2022-05-21T15:28:19Z"",""endDate"":""2022-06-21T15:28:19Z""}",
            @"{""type"":""Super"",""startDate"":""2022-05-22T15:28:19Z"",""endDate"":""2023-05-22T15:28:19Z""}",
            @"{""type"":""Super"",""startDate"":""2022-05-23T15:28:19Z"",""endDate"":""2023-05-23T15:28:19Z""}",
        };

        // Act
        var responses = new List<HttpResponseMessage>();
        foreach (var body in requestBodies)
        {
            var response = await _fixture.UserApiHttpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Post, "/subscription")
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json"),
                });
            responses.Add(response);
        }

        // Assert
        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));

        await using var assertDbContext = _fixture.GetDbContext();
        var subscriptions = await assertDbContext.Subscriptions.OrderBy(s => s.Id).ToArrayAsync();
        Assert.Equal(7, subscriptions.Length);

        Assert.Equal(1, subscriptions[0].Id);
        Assert.Equal("Free", subscriptions[0].Type);
        Assert.Equal(DateTime.Parse("2022-05-17T15:28:19Z").ToUniversalTime(), subscriptions[0].StartDate);
        Assert.Equal(DateTime.Parse("2099-01-01T00:00:00Z").ToUniversalTime(), subscriptions[0].EndDate);

        Assert.Equal(2, subscriptions[1].Id);
        Assert.Equal("Super", subscriptions[1].Type);
        Assert.Equal(DateTime.Parse("2022-05-18T15:28:19Z").ToUniversalTime(), subscriptions[1].StartDate);
        Assert.Equal(DateTime.Parse("2022-08-18T15:28:19Z").ToUniversalTime(), subscriptions[1].EndDate);

        Assert.Equal(3, subscriptions[2].Id);
        Assert.Equal("Trial", subscriptions[2].Type);
        Assert.Equal(DateTime.Parse("2022-05-19T15:28:19Z").ToUniversalTime(), subscriptions[2].StartDate);
        Assert.Equal(DateTime.Parse("2022-06-19T15:28:19Z").ToUniversalTime(), subscriptions[2].EndDate);

        Assert.Equal(4, subscriptions[3].Id);
        Assert.Equal("Free", subscriptions[3].Type);
        Assert.Equal(DateTime.Parse("2022-05-20T15:28:19Z").ToUniversalTime(), subscriptions[3].StartDate);
        Assert.Equal(DateTime.Parse("2099-01-01T00:00:00Z").ToUniversalTime(), subscriptions[3].EndDate);

        Assert.Equal(5, subscriptions[4].Id);
        Assert.Equal("Trial", subscriptions[4].Type);
        Assert.Equal(DateTime.Parse("2022-05-21T15:28:19Z").ToUniversalTime(), subscriptions[4].StartDate);
        Assert.Equal(DateTime.Parse("2022-06-21T15:28:19Z").ToUniversalTime(), subscriptions[4].EndDate);

        Assert.Equal(6, subscriptions[5].Id);
        Assert.Equal("Super", subscriptions[5].Type);
        Assert.Equal(DateTime.Parse("2022-05-22T15:28:19Z").ToUniversalTime(), subscriptions[5].StartDate);
        Assert.Equal(DateTime.Parse("2023-05-22T15:28:19Z").ToUniversalTime(), subscriptions[5].EndDate);

        Assert.Equal(7, subscriptions[6].Id);
        Assert.Equal("Super", subscriptions[6].Type);
        Assert.Equal(DateTime.Parse("2022-05-23T15:28:19Z").ToUniversalTime(), subscriptions[6].StartDate);
        Assert.Equal(DateTime.Parse("2023-05-23T15:28:19Z").ToUniversalTime(), subscriptions[6].EndDate);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 5)]
    [InlineData(3, 6)]
    [TestPriority(3)]
    public async Task UserApi_SetSubscriptionId_SetsUserSubscriptionId(int userId, int subscriptionId)
    {
        // Act
        var response = await _fixture.UserApiHttpClient.PatchAsync(
            $"/user/{userId}/subscriptionId/{subscriptionId}", content: null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var assertDbContext = _fixture.GetDbContext();
        var user = await assertDbContext.Users.Where(u => u.Id == userId).SingleAsync();
        Assert.Equal(subscriptionId, user.SubscriptionId);
    }

    [Fact]
    [TestPriority(4)]
    public async Task ProjectApi_CreateProject_AddsProjectsToDb()
    {
        // Arrange
        #region Request bodies

        var requestBodies = new[]
        {
            """
{
    "userId": 3,
    "name": "my super project 1",
    "charts": [
        {
            "symbol": "EURUSD",
            "timeframe": "M5",
            "indicators": []
        },
        {
            "symbol": "USDJPY",
            "timeframe": "H1",
            "indicators": [
                {
                    "name": "BB",
                    "parameters": "a=1;b=2;c=3"
                },
                {
                    "name": "MA",
                    "parameters": "a=1;b=2;c=3"
                }
            ]
        }
    ]
}
""",
            """
{
    "userId": 3,
    "name": "my super project 2",
    "charts": [
        {
            "symbol": "EURUSD",
            "timeframe": "M5",
            "indicators": [
                {
                    "name": "MA",
                    "parameters" : "a=1;b=2;c=3"
                }
             ]
        }
    ]
}
""",
            """
{
    "userId": 3,
    "name": "my super project 3",
    "charts": []
}
""",
            """
{
    "userId": 2,
    "name": "project 1",
    "charts": [
        {
            "symbol": "EURUSD",
            "timeframe": "H1",
            "indicators": [
                {
                    "name": "RSI",
                    "parameters" : "a=1;b=2;c=3"
                }
            ]
        }
    ]
}
""",
            """
{
    "userId": 2,
    "name": "project 2",
    "charts": [
        {
            "symbol": "USDJPY",
            "timeframe": "H1",
            "indicators": [
                {
                    "name": "MA",
                    "parameters" : "a=1;b=2;c=3"
                }
            ]
        }
    ]
}
""",
            """
{
    "userId": 1,
    "name": "project 3",
    "charts": [
        {
            "symbol": "EURUSD",
            "timeframe": "M5",
            "indicators": [
                {
                    "name": "RSI",
                    "parameters" : "a=1;b=2;c=3"
                },
                {
                    "name": "MA",
                    "parameters" : "a=1;b=2;c=3"
                }
            ]
        }
    ]
}
"""
        };

        #endregion

        // Act
        var responses = new List<HttpResponseMessage>();
        foreach (var body in requestBodies)
        {
            var response = await _fixture.ProjectApiHttpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Post, "/project")
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json"),
                });
            responses.Add(response);
        }

        // Assert
        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));

        // TODO add asserts for stored entities
    }

    [Theory]
    [InlineData("Free", "[]")]
    [InlineData("Trial", """[{"name":"RSI","used":1},{"name":"MA","used":1}]""")]
    [InlineData("Super", """[{"name":"MA","used":3},{"name":"RSI","used":1},{"name":"BB","used":1}]""")]
    [TestPriority(5)]
    public async Task ProjectApi_PopularIndicators_ReturnsExpectedResult(string subscriptionType, string expectedResponse)
    {
        // Act
        var response = await _fixture.ProjectApiHttpClient.GetAsync($"popularIndicators/{subscriptionType}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse, await response.Content.ReadAsStringAsync());
    }
}
