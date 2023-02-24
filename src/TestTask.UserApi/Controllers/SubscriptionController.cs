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
public class SubscriptionController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISubscriptionStore _subscriptionStore;

    public SubscriptionController(IMapper mapper, ISubscriptionStore subscriptionStore)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _subscriptionStore = subscriptionStore ?? throw new ArgumentNullException(nameof(subscriptionStore));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var subscription = _mapper.Map<Models.Subscription>(request);
        await _subscriptionStore.AddAsync(subscription, cancellationToken);
        var response = _mapper.Map<CreateSubscriptionResponse>(subscription);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionStore.GetAsync(id, cancellationToken);

        if (subscription == null)
        {
            return NotFound();
        }

        var response = _mapper.Map<GetSubscriptionResponse>(subscription);
        return Ok(response);
    }
}