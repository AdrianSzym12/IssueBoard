using IssueBoard.Web.Models.Auth;
using IssueBoard.Web.Services;

namespace IssueBoard.Web.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IApiClient _apiClient;
    private readonly JwtAuthenticationStateProvider _authenticationStateProvider;

    public AuthService(
        IApiClient apiClient,
        JwtAuthenticationStateProvider authenticationStateProvider)
    {
        _apiClient = apiClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<ApiResult<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ApiResult<AuthResponse> result = await _apiClient.PostAsync<LoginRequest, AuthResponse>(
            "api/auth/login",
            request,
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            await _authenticationStateProvider.MarkUserAsAuthenticatedAsync(
                result.Value.AccessToken);
        }

        return result;
    }

    public async Task<ApiResult<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        ApiResult<AuthResponse> result = await _apiClient.PostAsync<RegisterRequest, AuthResponse>(
            "api/auth/register",
            request,
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            await _authenticationStateProvider.MarkUserAsAuthenticatedAsync(
                result.Value.AccessToken);
        }

        return result;
    }

    public Task LogoutAsync()
    {
        return _authenticationStateProvider.MarkUserAsLoggedOutAsync();
    }
}
