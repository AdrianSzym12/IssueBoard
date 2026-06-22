using FluentValidation;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Workspaces.Members.Add;

public sealed class AddWorkspaceMemberCommandValidator : AbstractValidator<AddWorkspaceMemberCommand>
{
    public AddWorkspaceMemberCommandValidator()
    {
        RuleFor(command => command.WorkspaceId)
            .NotEmpty();

        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

        RuleFor(command => command.Role)
            .IsInEnum()
            .Must(role => role != WorkspaceRole.Owner)
            .WithMessage("Use role change operation to promote a user to Owner.");

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
