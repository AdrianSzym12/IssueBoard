using FluentValidation;

namespace IssueBoard.Application.Projects.Create;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.WorkspaceId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.Key)
            .NotEmpty()
            .MaximumLength(12)
            .Matches("^[A-Za-z][A-Za-z0-9-]{1,11}$")
            .WithMessage("Project key must start with a letter and contain only letters, numbers or hyphens.");

        RuleFor(command => command.Description)
            .MaximumLength(1000);

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
