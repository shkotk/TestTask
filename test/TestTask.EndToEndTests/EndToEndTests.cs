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
    public async Task UserApi_CreateUserRequests_AddsUsersToDb()
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
            var request = new HttpRequestMessage(HttpMethod.Post, "/user");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            // I have no idea why, but all calls to http client return 404.
            // I've even tried to use good old Startup/Program definitions (without top-level statements) but it didn't work either.
            // Either WebApplicationFactory is broken, or I'm missing something.
            var response = await _fixture.UserApiHttpClient.SendAsync(request);
            responses.Add(response);
        }
        
        // Assert
        await using var assertDbContext = _fixture.GetDbContext();
        var users = await assertDbContext.Users.OrderBy(u => u.Id).ToArrayAsync();
        Assert.Equal(3, users.Length);
        
        Assert.Equal(HttpStatusCode.OK, responses[0].StatusCode);
        Assert.Equal(1, users[0].Id);
        Assert.Equal("John Doe", users[0].Name);
        Assert.Equal("John@example.com", users[0].Email);
        Assert.Null(users[0].SubscriptionId);
        
        Assert.Equal(HttpStatusCode.OK, responses[1].StatusCode);
        Assert.Equal(2, users[1].Id);
        Assert.Equal("Mark Shimko", users[1].Name);
        Assert.Equal("Mark@example.com", users[1].Email);
        Assert.Null(users[1].SubscriptionId);
        
        Assert.Equal(HttpStatusCode.OK, responses[2].StatusCode);
        Assert.Equal(3, users[2].Id);
        Assert.Equal("Taras Ovruch", users[2].Name);
        Assert.Equal("Taras@example.com", users[2].Email);
        Assert.Null(users[3].SubscriptionId);
    }
}