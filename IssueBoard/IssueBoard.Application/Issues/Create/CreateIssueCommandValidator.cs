using FluentValidation;

namespace IssueBoard.Application.Issues.Create;

public sealed class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(4000);

        RuleFor(command => command.Priority)
            .IsInEnum();

        RuleFor(command => command.AssigneeUserId)
            .Must(assigneeUserId => assigneeUserId is null || assigneeUserId.Value != Guid.Empty)
            .WithMessage("Assignee user id cannot be empty.");

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
