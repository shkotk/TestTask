using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestTask.ProjectApi.Interfaces;
using TestTask.ProjectApi.Models;

namespace TestTask.ProjectApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectStore _projectStore;

    public ProjectController(IProjectStore projectStore)
    {
        _projectStore = projectStore ?? throw new ArgumentNullException(nameof(projectStore));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Project project, CancellationToken cancellationToken)
    {
        await _projectStore.Add(project, cancellationToken);
        return Ok(new {project.Id});
    }

    [HttpGet("byUserId/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var result = await _projectStore.GetByUserId(userId, cancellationToken);

        if (result == null || result.Count == 0)
        {
            return NotFound();
        }

        return Ok(result);
    }
}