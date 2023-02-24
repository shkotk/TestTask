using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TestTask.UserApi.ApiModels;
using TestTask.UserApi.Controllers;
using TestTask.UserApi.Interfaces;
using TestTask.UserApi.Models;
using Xunit;

namespace TestTask.UserApi.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserStore> _userStoreMock;

    private readonly UserController _controller;

    public UserControllerTests()
    {
        var mapper = new ServiceCollection()
            .AddAutoMapper(typeof(MappingProfile))
            .BuildServiceProvider()
            .GetRequiredService<IMapper>();
        _userStoreMock = new Mock<IUserStore>();

        _controller = new UserController(mapper, _userStoreMock.Object);
    }

    [Fact]
    public async Task Get_UserExists_ReturnsOk()
    {
        // Arrange
        _userStoreMock.Setup(store => store.GetAsync(25, CancellationToken.None))
            .ReturnsAsync(new User {Id = 25, Name = "foo", Email = "foo@bar.com", SubscriptionId = 11});

        // Act
        var result = await _controller.Get(25, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<GetUserResponse>(okResult.Value);

        Assert.Equal(25, response.Id);
        Assert.Equal("foo", response.Name);
        Assert.Equal("foo@bar.com", response.Email);
        Assert.Equal(11, response.SubscriptionId);

        _userStoreMock.Verify(store => store.GetAsync(25, CancellationToken.None), Times.Once);
        _userStoreMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_UserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _userStoreMock.Setup(store => store.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User) null);

        // Act
        var result = await _controller.Get(25, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<NotFoundResult>(result);

        _userStoreMock.Verify(store => store.GetAsync(25, CancellationToken.None), Times.Once);
        _userStoreMock.VerifyNoOtherCalls();
    }
}