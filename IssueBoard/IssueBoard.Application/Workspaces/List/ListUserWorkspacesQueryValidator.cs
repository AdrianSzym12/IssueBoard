using FluentValidation;

namespace IssueBoard.Application.Workspaces.List;

public sealed class ListUserWorkspacesQueryValidator : AbstractValidator<ListUserWorkspacesQuery>
{
    public ListUserWorkspacesQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEmpty();
    }
}
