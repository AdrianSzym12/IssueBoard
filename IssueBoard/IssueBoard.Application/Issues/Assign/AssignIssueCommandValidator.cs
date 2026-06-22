using FluentValidation;

namespace IssueBoard.Application.Issues.Assign;

public sealed class AssignIssueCommandValidator : AbstractValidator<AssignIssueCommand>
{
    public AssignIssueCommandValidator()
    {
        RuleFor(command => command.IssueId)
            .NotEmpty();

        RuleFor(command => command.AssigneeUserId)
            .Must(assigneeUserId => assigneeUserId is null || assigneeUserId.Value != Guid.Empty)
            .WithMessage("Assignee user id cannot be empty.");

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
