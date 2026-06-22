using FluentValidation;

namespace IssueBoard.Application.Workspaces.Members.List;

public sealed class ListWorkspaceMembersQueryValidator : AbstractValidator<ListWorkspaceMembersQuery>
{
    public ListWorkspaceMembersQueryValidator()
    {
        RuleFor(query => query.WorkspaceId)
            .NotEmpty();

        RuleFor(query => query.RequesterUserId)
            .NotEmpty();
    }
}
