using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Common.Pagination;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.List;

public sealed class ListProjectIssuesQueryHandler
    : IQueryHandler<ListProjectIssuesQuery, PagedList<IssueDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IIssueRepository _issueRepository;

    public ListProjectIssuesQueryHandler(
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        IIssueRepository issueRepository)
    {
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _issueRepository = issueRepository;
    }

    public async Task<Result<PagedList<IssueDto>>> Handle(
        ListProjectIssuesQuery request,
        CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<PagedList<IssueDto>>.Failure(
                IssueErrors.ProjectNotFound(request.ProjectId));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<PagedList<IssueDto>>.Failure(
                IssueErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!IssueAuthorization.CanViewIssues(workspace, request.RequesterUserId))
        {
            return Result<PagedList<IssueDto>>.Failure(
                IssueErrors.NotWorkspaceMember());
        }

        string sortBy = string.IsNullOrWhiteSpace(request.SortBy)
            ? "updatedAtUtc"
            : request.SortBy.Trim();

        IssueSearchOptions options = new(
            request.ProjectId,
            request.Status,
            request.Priority,
            request.AssigneeUserId,
            request.SearchTerm,
            sortBy,
            request.SortDescending,
            request.PageNumber,
            request.PageSize);

        PagedList<Issue> pagedIssues = await _issueRepository.SearchAsync(
            options,
            cancellationToken);

        IReadOnlyList<IssueDto> issueDtos = pagedIssues.Items
            .Select(issue => issue.ToDto())
            .ToList();

        PagedList<IssueDto> response = new(
            issueDtos,
            pagedIssues.PageNumber,
            pagedIssues.PageSize,
            pagedIssues.TotalCount);

        return Result<PagedList<IssueDto>>.Success(response);
    }
}
