using FluentValidation;

namespace IssueBoard.Application.Issues.List;

public sealed class ListProjectIssuesQueryValidator : AbstractValidator<ListProjectIssuesQuery>
{
    private static readonly string[] AllowedSortFields =
    [
        "number",
        "title",
        "status",
        "priority",
        "dueDate",
        "dueDateUtc",
        "created",
        "createdAtUtc",
        "updated",
        "updatedAtUtc"
    ];

    public ListProjectIssuesQueryValidator()
    {
        RuleFor(query => query.ProjectId)
            .NotEmpty();

        RuleFor(query => query.Status)
            .IsInEnum()
            .When(query => query.Status.HasValue);

        RuleFor(query => query.Priority)
            .IsInEnum()
            .When(query => query.Priority.HasValue);

        RuleFor(query => query.AssigneeUserId)
            .Must(assigneeUserId => assigneeUserId is null || assigneeUserId.Value != Guid.Empty)
            .WithMessage("Assignee user id cannot be empty.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200);

        RuleFor(query => query.SortBy)
            .Must(BeAllowedSortField)
            .WithMessage("Sort by must be one of: number, title, status, priority, dueDate, dueDateUtc, created, createdAtUtc, updated, updatedAtUtc.");

        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }

    private static bool BeAllowedSortField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return true;
        }

        return AllowedSortFields.Contains(
            sortBy.Trim(),
            StringComparer.OrdinalIgnoreCase);
    }
}
