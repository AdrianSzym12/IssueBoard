using IssueBoard.Web.Models.Auth;
using IssueBoard.Web.Services;

namespace IssueBoard.Web.Auth;

public interface IAuthService
{
    Task<ApiResult<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task LogoutAsync();
}
