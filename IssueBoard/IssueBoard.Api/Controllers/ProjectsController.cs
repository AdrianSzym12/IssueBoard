using IssueBoard.Api.Contracts.Projects;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Projects.Archive;
using IssueBoard.Application.Projects.Create;
using IssueBoard.Application.Projects.Get;
using IssueBoard.Application.Projects.List;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueBoard.Api.Controllers;

[Authorize]
[Route("api")]
public sealed class ProjectsController : ApiControllerBase
{
    private readonly ISender _sender;

    public ProjectsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("workspaces/{workspaceId:guid}/projects")]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListWorkspaceProjects(
        Guid workspaceId,
        [FromQuery] bool includeArchived,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ListWorkspaceProjectsQuery query = new(
            workspaceId,
            currentUserId,
            includeArchived);

        Result<IReadOnlyList<ProjectDto>> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("workspaces/{workspaceId:guid}/projects")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProject(
        Guid workspaceId,
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        CreateProjectCommand command = new(
            workspaceId,
            request.Name,
            request.Key,
            request.Description,
            currentUserId);

        Result<ProjectDto> result = await _sender.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("projects/{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        GetProjectByIdQuery query = new(
            projectId,
            currentUserId);

        Result<ProjectDto> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("projects/{projectId:guid}/archive")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ArchiveProjectCommand command = new(
            projectId,
            currentUserId);

        Result<ProjectDto> result = await _sender.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }
}
