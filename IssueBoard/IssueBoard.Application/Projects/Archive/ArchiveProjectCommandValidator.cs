using FluentValidation;

namespace IssueBoard.Application.Projects.Archive;

public sealed class ArchiveProjectCommandValidator : AbstractValidator<ArchiveProjectCommand>
{
    public ArchiveProjectCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
