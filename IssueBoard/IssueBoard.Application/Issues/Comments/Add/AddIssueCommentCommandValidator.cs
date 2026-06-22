using FluentValidation;

namespace IssueBoard.Application.Issues.Comments.Add;

public sealed class AddIssueCommentCommandValidator : AbstractValidator<AddIssueCommentCommand>
{
    public AddIssueCommentCommandValidator()
    {
        RuleFor(command => command.IssueId)
            .NotEmpty();

        RuleFor(command => command.Content)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(command => command.RequesterUserId)
            .NotEmpty();
    }
}
