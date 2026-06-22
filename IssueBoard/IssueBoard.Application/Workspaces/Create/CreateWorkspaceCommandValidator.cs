using FluentValidation;

namespace IssueBoard.Application.Workspaces.Create;

public sealed class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.Description)
            .MaximumLength(500);

        RuleFor(command => command.OwnerUserId)
            .NotEmpty();
    }
}
