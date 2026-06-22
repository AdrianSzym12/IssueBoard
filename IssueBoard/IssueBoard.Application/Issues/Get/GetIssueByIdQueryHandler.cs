using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Get;

public sealed class GetIssueByIdQueryHandler
    : IQueryHandler<GetIssueByIdQuery, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;

    public GetIssueByIdQueryHandler(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<IssueDto>> Handle(
        GetIssueByIdQuery request,
        CancellationToken cancellationToken)
    {
        Issue? issue = await _issueRepository.GetByIdAsync(
            request.IssueId,
            cancellationToken);

        if (issue is null)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.NotFound(request.IssueId));
        }

        Project? project = await _projectRepository.GetByIdAsync(
            issue.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.ProjectNotFound(issue.ProjectId));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!IssueAuthorization.CanViewIssues(workspace, request.RequesterUserId))
        {
            return Result<IssueDto>.Failure(
                IssueErrors.NotWorkspaceMember());
        }

        return Result<IssueDto>.Success(issue.ToDto());
    }
}
