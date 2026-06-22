using FluentValidation;

namespace IssueBoard.Application.Issues.Update;

public sealed class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleFor(command => command.IssueId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(4000);

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
