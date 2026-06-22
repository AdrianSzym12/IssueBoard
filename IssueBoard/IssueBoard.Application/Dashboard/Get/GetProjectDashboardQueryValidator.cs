using FluentValidation;

namespace IssueBoard.Application.Dashboard.Get;

public sealed class GetProjectDashboardQueryValidator : AbstractValidator<GetProjectDashboardQuery>
{
    public GetProjectDashboardQueryValidator()
    {
        RuleFor(query => query.ProjectId)
            .NotEmpty();

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }
}
