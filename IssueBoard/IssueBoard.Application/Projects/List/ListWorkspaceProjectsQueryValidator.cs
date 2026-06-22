using FluentValidation;

namespace IssueBoard.Application.Projects.List;

public sealed class ListWorkspaceProjectsQueryValidator
    : AbstractValidator<ListWorkspaceProjectsQuery>
{
    public ListWorkspaceProjectsQueryValidator()
    {
        RuleFor(query => query.WorkspaceId)
            .NotEmpty();

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }
}
