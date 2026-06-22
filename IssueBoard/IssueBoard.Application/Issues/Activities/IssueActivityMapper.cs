using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Activities;

internal static class IssueActivityMapper
{
    public static IssueActivityDto ToDto(this IssueActivity activity)
    {
        return new IssueActivityDto(
            activity.Id,
            activity.IssueId,
            activity.ActorUserId,
            activity.Action,
            activity.OldValue,
            activity.NewValue,
            activity.CreatedAtUtc);
    }
}
