using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Projects;

internal static class ProjectMapper
{
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto(
            project.Id,
            project.WorkspaceId,
            project.Name,
            project.Key,
            project.Description,
            project.IsArchived,
            project.CreatedAtUtc,
            project.UpdatedAtUtc);
    }
}
