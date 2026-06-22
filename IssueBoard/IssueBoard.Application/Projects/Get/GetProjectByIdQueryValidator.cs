using FluentValidation;

namespace IssueBoard.Application.Projects.Get;

public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(query => query.ProjectId)
            .NotEmpty();

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }
}
