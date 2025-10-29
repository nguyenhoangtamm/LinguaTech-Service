using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class ModulesController(ILogger<ModulesController> logger, IModuleService moduleService) : ApiControllerBase(logger)
{
    // POST /api/v1/modules/create
    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateModuleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating module with title: {request.Title}");

            return await moduleService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating module", ex);
            return StatusCode(500, "An error occurred while creating the module");
        }
    }

    // POST /api/v1/modules/update/{id}
    [HttpPost("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateModuleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating module with ID: {id}");

            return await moduleService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating module with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the module");
        }
    }

    // POST /api/v1/modules/delete/{id}
    [HttpPost("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting module with ID: {id}");

            return await moduleService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting module with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the module");
        }
    }

    // GET /api/v1/modules/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ModuleWithLessonsType>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting module with ID: {id}");

            return await moduleService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting module with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the module");
        }
    }
}