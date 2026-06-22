using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dashboard.Get;
using IssueBoard.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueBoard.Api.Controllers;

[Authorize]
[Route("api/projects/{projectId:guid}/dashboard")]
public sealed class ProjectDashboardController : ApiControllerBase
{
    private readonly ISender _sender;

    public ProjectDashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProjectDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectDashboard(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        GetProjectDashboardQuery query = new(
            projectId,
            currentUserId);

        Result<ProjectDashboardDto> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }
}
