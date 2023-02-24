using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestTask.UserApi.ApiModels;
using TestTask.UserApi.Interfaces;

namespace TestTask.UserApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserStore _userStore;

    public UserController(IMapper mapper, IUserStore userStore)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<Models.User>(request);
        await _userStore.AddAsync(user, cancellationToken);
        var response = _mapper.Map<CreateUserResponse>(user);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var user = await _userStore.GetAsync(id, cancellationToken);

        if (user == null)
        {
            return NotFound();
        }

        var response = _mapper.Map<GetUserResponse>(user);
        return Ok(response);
    }

    [HttpPatch("{userId:int}/subscriptionId/{subscriptionId:int}")]
    public async Task<IActionResult> SetSubscriptionId(
        [FromRoute] int userId, [FromRoute] int subscriptionId, CancellationToken cancellationToken)
    {
        await _userStore.SetSubscriptionId(userId, subscriptionId, cancellationToken);
        return Ok();
    }

    [HttpGet("subscriptionTypes")]
    public async Task<IActionResult> GetSubscriptionTypes(
        [FromBody] GetSubscriptionTypesRequest request,
        CancellationToken cancellationToken)
    {
        var subscriptionTypes = await _userStore.GetSubscriptionTypes(request.UserIds, cancellationToken);
        return Ok(new GetSubscriptionTypesResponse {SubscriptionTypes = subscriptionTypes});
    }
}