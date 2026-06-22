using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Projects.Archive;

public sealed record ArchiveProjectCommand(
    Guid ProjectId,
    Guid RequesterUserId) : ICommand<ProjectDto>;
