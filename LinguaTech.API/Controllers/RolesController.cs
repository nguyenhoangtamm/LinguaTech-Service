using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RolesController(ILogger<RolesController> logger, IRoleService roleService) : ApiControllerBase(logger)
{
    private readonly IRoleService _roleService = roleService;

    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating role with name: {request.Name}");

            return await _roleService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating role", ex);
            return StatusCode(500, "An error occurred while creating the role");
        }
    }

    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating role with ID: {request.Id}");

            return await _roleService.Update(request.Id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating role with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the role");
        }
    }

    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting role with ID: {id}");

            return await _roleService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting role with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the role");
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<GetRoleDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting role with ID: {id}");

            return await _roleService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting role with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the role");
        }
    }

    [HttpGet("get-all")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<GetAllRolesDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all roles");

            return await _roleService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all roles", ex);
            return StatusCode(500, "An error occurred while retrieving roles");
        }
    }

    [HttpGet("get-pagination")]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<GetRolesWithPaginationDto>>> GetRolesWithPagination([FromQuery] GetRolesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting roles with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await _roleService.GetRolesWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting roles with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving roles");
        }
    }
}