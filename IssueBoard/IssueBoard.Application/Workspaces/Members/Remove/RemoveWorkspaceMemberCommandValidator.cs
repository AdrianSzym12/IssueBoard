using FluentValidation;

namespace IssueBoard.Application.Workspaces.Members.Remove;

public sealed class RemoveWorkspaceMemberCommandValidator
    : AbstractValidator<RemoveWorkspaceMemberCommand>
{
    public RemoveWorkspaceMemberCommandValidator()
    {
        RuleFor(command => command.WorkspaceId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
