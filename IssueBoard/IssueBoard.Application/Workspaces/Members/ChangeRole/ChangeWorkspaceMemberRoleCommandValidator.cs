using FluentValidation;

namespace IssueBoard.Application.Workspaces.Members.ChangeRole;

public sealed class ChangeWorkspaceMemberRoleCommandValidator
    : AbstractValidator<ChangeWorkspaceMemberRoleCommand>
{
    public ChangeWorkspaceMemberRoleCommandValidator()
    {
        RuleFor(command => command.WorkspaceId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.Role)
            .IsInEnum();

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
