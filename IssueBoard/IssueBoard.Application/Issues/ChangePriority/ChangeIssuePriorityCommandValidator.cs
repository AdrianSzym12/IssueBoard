using FluentValidation;

namespace IssueBoard.Application.Issues.ChangePriority;

public sealed class ChangeIssuePriorityCommandValidator
    : AbstractValidator<ChangeIssuePriorityCommand>
{
    public ChangeIssuePriorityCommandValidator()
    {
        RuleFor(command => command.IssueId)
            .NotEmpty();

        RuleFor(command => command.Priority)
            .IsInEnum();

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
