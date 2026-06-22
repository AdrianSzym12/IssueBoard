using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Auth.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<AuthResponseDto>;
