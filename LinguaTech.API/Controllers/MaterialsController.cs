using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class MaterialsController(ILogger<MaterialsController> logger, IMaterialService materialService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateMaterialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating material with filename: {request.FileName}");

            var result = await materialService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating material", ex);
            return StatusCode(500, "An error occurred while creating the material");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateMaterialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating material with ID: {request.Id}");

            var result = await materialService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating material with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the material");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting material with ID: {id}");

            var result = await materialService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting material with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the material");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting material with ID: {id}");

            var result = await materialService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting material with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the material");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all materials");

            var result = await materialService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all materials", ex);
            return StatusCode(500, "An error occurred while retrieving materials");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetMaterialsWithPagination([FromQuery] GetMaterialsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting materials with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await materialService.GetMaterialsWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting materials with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving materials");
        }
    }
}