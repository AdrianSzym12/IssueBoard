using FluentValidation;

namespace IssueBoard.Application.Issues.Get;

public sealed class GetIssueByIdQueryValidator : AbstractValidator<GetIssueByIdQuery>
{
    public GetIssueByIdQueryValidator()
    {
        RuleFor(query => query.IssueId)
            .NotEmpty();

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }
}
