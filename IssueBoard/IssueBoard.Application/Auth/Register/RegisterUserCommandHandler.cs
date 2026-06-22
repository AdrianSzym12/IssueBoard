using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Abstractions.Security;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Auth.Register;

public sealed class RegisterUserCommandHandler
    : ICommandHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        bool emailAlreadyExists = await _userRepository.ExistsByEmailAsync(
            request.Email,
            cancellationToken);

        if (emailAlreadyExists)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Conflict(
                    "Users.EmailAlreadyExists",
                    "User with this email address already exists."));
        }

        string passwordHash = _passwordHasher.HashPassword(request.Password);

        User user = User.Create(
            request.Email,
            request.DisplayName,
            passwordHash);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        JwtTokenResult token = _jwtTokenGenerator.GenerateToken(user);

        AuthResponseDto response = new(
            user.Id,
            user.Email,
            user.DisplayName,
            token.AccessToken,
            token.ExpiresAtUtc);

        return Result<AuthResponseDto>.Success(response);
    }
}
