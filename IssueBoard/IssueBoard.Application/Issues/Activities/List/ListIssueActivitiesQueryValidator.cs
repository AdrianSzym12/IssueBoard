using FluentValidation;

namespace IssueBoard.Application.Issues.Activities.List;

public sealed class ListIssueActivitiesQueryValidator : AbstractValidator<ListIssueActivitiesQuery>
{
    public ListIssueActivitiesQueryValidator()
    {
        RuleFor(query => query.IssueId)
            .NotEmpty();

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }
}
