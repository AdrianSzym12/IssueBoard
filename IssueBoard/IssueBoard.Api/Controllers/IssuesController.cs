using IssueBoard.Api.Contracts.Issues;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Issues.Assign;
using IssueBoard.Application.Issues.ChangePriority;
using IssueBoard.Application.Issues.ChangeStatus;
using IssueBoard.Application.Issues.Comments.Add;
using IssueBoard.Application.Issues.Comments.List;
using IssueBoard.Application.Issues.Create;
using IssueBoard.Application.Issues.Get;
using IssueBoard.Application.Issues.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IssueBoard.Application.Issues.Activities.List;
using IssueBoard.Application.Common.Pagination;
using IssueBoard.Application.Issues.List;

namespace IssueBoard.Api.Controllers;

[Authorize]
[Route("api")]
public sealed class IssuesController : ApiControllerBase
{
    private readonly ISender _sender;

    public IssuesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("projects/{projectId:guid}/issues")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateIssue(
        Guid projectId,
        CreateIssueRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        CreateIssueCommand command = new(
            projectId,
            request.Title,
            request.Description,
            request.Priority,
            request.AssigneeUserId,
            request.DueDateUtc,
            currentUserId);

        Result<IssueDto> result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("projects/{projectId:guid}/issues")]
    [ProducesResponseType(typeof(PagedList<IssueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListProjectIssues(
    Guid projectId,
    [FromQuery] SearchIssuesRequest request,
    CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ListProjectIssuesQuery query = new(
            projectId,
            request.Status,
            request.Priority,
            request.AssigneeUserId,
            request.SearchTerm,
            request.SortBy,
            request.SortDescending,
            request.PageNumber,
            request.PageSize,
            currentUserId);

        Result<PagedList<IssueDto>> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("issues/{issueId:guid}")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIssue(
        Guid issueId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        GetIssueByIdQuery query = new(issueId, currentUserId);

        Result<IssueDto> result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }
    [HttpGet("issues/{issueId:guid}/activities")]
    [ProducesResponseType(typeof(IReadOnlyList<IssueActivityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListActivities(
    Guid issueId,
    CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ListIssueActivitiesQuery query = new(
            issueId,
            currentUserId);

        Result<IReadOnlyList<IssueActivityDto>> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }
    [HttpPut("issues/{issueId:guid}")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateIssue(
        Guid issueId,
        UpdateIssueRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        UpdateIssueCommand command = new(
            issueId,
            request.Title,
            request.Description,
            request.DueDateUtc,
            currentUserId);

        Result<IssueDto> result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("issues/{issueId:guid}/status")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangeStatus(
        Guid issueId,
        ChangeIssueStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ChangeIssueStatusCommand command = new(
            issueId,
            request.Status,
            currentUserId);

        Result<IssueDto> result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("issues/{issueId:guid}/priority")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangePriority(
        Guid issueId,
        ChangeIssuePriorityRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ChangeIssuePriorityCommand command = new(
            issueId,
            request.Priority,
            currentUserId);

        Result<IssueDto> result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("issues/{issueId:guid}/assignee")]
    [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignIssue(
        Guid issueId,
        AssignIssueRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        AssignIssueCommand command = new(
            issueId,
            request.AssigneeUserId,
            currentUserId);

        Result<IssueDto> result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("issues/{issueId:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<IssueCommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListComments(
        Guid issueId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        ListIssueCommentsQuery query = new(
            issueId,
            currentUserId);

        Result<IReadOnlyList<IssueCommentDto>> result = await _sender.Send(
            query,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("issues/{issueId:guid}/comments")]
    [ProducesResponseType(typeof(IssueCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddComment(
        Guid issueId,
        AddIssueCommentRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid currentUserId))
        {
            return MissingUserIdClaim();
        }

        AddIssueCommentCommand command = new(
            issueId,
            request.Content,
            currentUserId);

        Result<IssueCommentDto> result = await _sender.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }
}
