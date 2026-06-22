using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues;

internal static class IssueMapper
{
    public static IssueDto ToDto(this Issue issue)
    {
        return new IssueDto(
            issue.Id,
            issue.ProjectId,
            issue.Number,
            issue.Title,
            issue.Description,
            issue.Status,
            issue.Priority,
            issue.CreatedByUserId,
            issue.AssigneeUserId,
            issue.DueDateUtc,
            issue.CreatedAtUtc,
            issue.UpdatedAtUtc);
    }
}
