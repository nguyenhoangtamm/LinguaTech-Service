using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class MaterialsController(ILogger<MaterialsController> logger, IMaterialService materialService) : ApiControllerBase(logger)
{
    // GET /api/v1/lessons/{lessonId}/materials
    [HttpGet("~/api/v1/lessons/{lessonId}/materials")]
    public async Task<ActionResult<Result<List<MaterialType>>>> GetMaterialsByLesson(int lessonId, [FromQuery] GetMaterialsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting materials for lesson ID: {lessonId}");

            return await materialService.GetByLessonId(lessonId, query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting materials for lesson ID: {lessonId}", ex);
            return StatusCode(500, "An error occurred while retrieving materials");
        }
    }

    // GET /api/v1/materials/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<MaterialType>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting material with ID: {id}");

            return await materialService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting material with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the material");
        }
    }

    // POST /api/v1/materials/create
    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateMaterialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating material with filename: {request.FileName}");

            return await materialService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating material", ex);
            return StatusCode(500, "An error occurred while creating the material");
        }
    }

    // POST /api/v1/materials/update/{id}
    [HttpPost("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateMaterialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating material with ID: {id}");

            return await materialService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating material with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the material");
        }
    }

    // POST /api/v1/materials/delete/{id}
    [HttpPost("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting material with ID: {id}");

            return await materialService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting material with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the material");
        }
    }

    // GET /api/v1/materials/{id}/download
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadMaterial(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Downloading material with ID: {id}");

            var result = await materialService.Download(id, cancellationToken);

            if (result.Succeeded)
            {
                return File(result.Data, "application/octet-stream", "download");
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error downloading material with ID: {id}", ex);
            return StatusCode(500, "An error occurred while downloading the material");
        }
    }
}