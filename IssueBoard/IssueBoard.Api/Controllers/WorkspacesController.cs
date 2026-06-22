using IssueBoard.Api.Contracts.Workspaces;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Workspaces.Create;
using IssueBoard.Application.Workspaces.List;
using IssueBoard.Application.Workspaces.Members.Add;
using IssueBoard.Application.Workspaces.Members.ChangeRole;
using IssueBoard.Application.Workspaces.Members.List;
using IssueBoard.Application.Workspaces.Members.Remove;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueBoard.Api.Controllers;

[Authorize]
[Route("api/workspaces")]
public sealed class WorkspacesController : ApiControllerBase
{
    private readonly ISender _sender;

    public WorkspacesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkspaceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListMyWorkspaces(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ListUserWorkspacesQuery query = new(currentUserId);

        Result<IReadOnlyList<WorkspaceDto>> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkspaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        CreateWorkspaceRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        CreateWorkspaceCommand command = new(
            request.Name,
            request.Description,
            currentUserId);

        Result<WorkspaceDto> result = await _sender.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{workspaceId:guid}/members")]
    [ProducesResponseType(typeof(IReadOnlyList<WorkspaceMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListMembers(
        Guid workspaceId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ListWorkspaceMembersQuery query = new(
            workspaceId,
            currentUserId);

        Result<IReadOnlyList<WorkspaceMemberDto>> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{workspaceId:guid}/members")]
    [ProducesResponseType(typeof(WorkspaceMemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddMember(
        Guid workspaceId,
        AddWorkspaceMemberRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        AddWorkspaceMemberCommand command = new(
            workspaceId,
            request.Email,
            request.Role,
            currentUserId);

        Result<WorkspaceMemberDto> result = await _sender.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{workspaceId:guid}/members/{userId:guid}/role")]
    [ProducesResponseType(typeof(WorkspaceMemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeMemberRole(
        Guid workspaceId,
        Guid userId,
        ChangeWorkspaceMemberRoleRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ChangeWorkspaceMemberRoleCommand command = new(
            workspaceId,
            userId,
            request.Role,
            currentUserId);

        Result<WorkspaceMemberDto> result = await _sender.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{workspaceId:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        RemoveWorkspaceMemberCommand command = new(
            workspaceId,
            userId,
            currentUserId);

        Result result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
