using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestTask.ProjectApi.Interfaces;

namespace TestTask.ProjectApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PopularIndicatorsController : ControllerBase
{
    private readonly IPopularIndicatorService _popularIndicatorService;

    public PopularIndicatorsController(IPopularIndicatorService popularIndicatorService)
    {
        _popularIndicatorService = popularIndicatorService ?? throw new ArgumentNullException(nameof(popularIndicatorService));
    }

    [HttpGet("{subscriptionType}")]
    public async Task<IActionResult> Get([FromRoute] string subscriptionType, CancellationToken cancellationToken)
    {
        return Ok(await _popularIndicatorService.Get(subscriptionType, top: 3, cancellationToken));
    }
}