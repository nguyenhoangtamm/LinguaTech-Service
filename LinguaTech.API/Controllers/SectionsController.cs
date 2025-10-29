using LinguaTech.Application.Services;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SectionsController(ILogger<SectionsController> logger, ISectionService sectionService) : ApiControllerBase(logger)
{
    private readonly ISectionService _sectionService = sectionService;

    // GET /api/v1/sections
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<Result<PaginatedResult<SectionType>>>> GetSections([FromQuery] GetSectionsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting sections with pagination - Page: {query.PageNumber}, Limit: {query.PageSize}");

            return await _sectionService.GetSectionsWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting sections with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving sections");
        }
    }

    // GET /api/v1/sections/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<SectionType>>> GetSection(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting section with ID: {id}");

            return await _sectionService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting section with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the section");
        }
    }

    // POST /api/v1/sections/create
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> CreateSection([FromBody] CreateSectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating section with title: {request.Title}");

            return await _sectionService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating section", ex);
            return StatusCode(500, "An error occurred while creating the section");
        }
    }

    // POST /api/v1/sections/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> UpdateSection([FromRoute] int id, [FromBody] UpdateSectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating section with ID: {id}");

            return await _sectionService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating section with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the section");
        }
    }

    // POST /api/v1/sections/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> DeleteSection([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting section with ID: {id}");

            return await _sectionService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting section with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the section");
        }
    }
}