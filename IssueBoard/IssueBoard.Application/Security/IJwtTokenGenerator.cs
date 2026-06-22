using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(User user);
}
