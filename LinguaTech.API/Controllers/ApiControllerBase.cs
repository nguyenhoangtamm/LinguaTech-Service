using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase(ILogger<ApiControllerBase> logger) : ControllerBase
{
    protected void LogInformation(string message)
    {
        logger.LogInformation(message);
    }

    protected void LogError(string message, Exception ex)
    {
        logger.LogError(ex, message);
    }
}