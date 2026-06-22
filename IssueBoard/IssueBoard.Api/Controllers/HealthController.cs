using Microsoft.AspNetCore.Mvc;

namespace IssueBoard.Api.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Application = "IssueBoard.Api",
            Status = "Healthy",
            TimestampUtc = DateTime.UtcNow
        });
    }
}
