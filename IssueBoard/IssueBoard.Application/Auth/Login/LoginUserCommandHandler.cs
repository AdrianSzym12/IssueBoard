using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Abstractions.Security;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Auth.Login;

public sealed class LoginUserCommandHandler
    : ICommandHandler<LoginUserCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.User? user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (user is null)
        {
            return InvalidCredentials();
        }

        if (!user.IsActive)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized(
                    "Auth.UserInactive",
                    "User account is inactive."));
        }

        bool passwordIsValid = _passwordHasher.VerifyPassword(
            user.PasswordHash,
            request.Password);

        if (!passwordIsValid)
        {
            return InvalidCredentials();
        }

        JwtTokenResult token = _jwtTokenGenerator.GenerateToken(user);

        AuthResponseDto response = new(
            user.Id,
            user.Email,
            user.DisplayName,
            token.AccessToken,
            token.ExpiresAtUtc);

        return Result<AuthResponseDto>.Success(response);
    }

    private static Result<AuthResponseDto> InvalidCredentials()
    {
        return Result<AuthResponseDto>.Failure(
            Error.Unauthorized(
                "Auth.InvalidCredentials",
                "Invalid email or password."));
    }
}
