using FluentValidation;

namespace IssueBoard.Application.Issues.Comments.List;

public sealed class ListIssueCommentsQueryValidator : AbstractValidator<ListIssueCommentsQuery>
{
    public ListIssueCommentsQueryValidator()
    {
        RuleFor(query => query.IssueId)
            .NotEmpty();

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }
}
