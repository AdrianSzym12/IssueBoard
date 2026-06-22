using FluentValidation;

namespace IssueBoard.Application.Issues.ChangeStatus;

public sealed class ChangeIssueStatusCommandValidator : AbstractValidator<ChangeIssueStatusCommand>
{
    public ChangeIssueStatusCommandValidator()
    {
        RuleFor(command => command.IssueId)
            .NotEmpty();

        RuleFor(command => command.Status)
            .IsInEnum();

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
