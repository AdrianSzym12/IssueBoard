using IssueBoard.Api.Contracts.Auth;
using IssueBoard.Application.Auth.Login;
using IssueBoard.Application.Auth.Register;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IssueBoard.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        RegisterUserCommand command = new(
            request.Email,
            request.DisplayName,
            request.Password);

        Result<AuthResponseDto> result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        LoginUserCommand command = new(
            request.Email,
            request.Password);

        Result<AuthResponseDto> result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
