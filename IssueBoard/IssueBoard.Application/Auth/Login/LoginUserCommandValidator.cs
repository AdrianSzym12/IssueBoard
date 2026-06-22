using FluentValidation;

namespace IssueBoard.Application.Auth.Login;

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty()
            .MaximumLength(100);
    }
}
