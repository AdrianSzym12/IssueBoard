using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Auth.Register;

public sealed record RegisterUserCommand(
    string Email,
    string DisplayName,
    string Password) : ICommand<AuthResponseDto>;
