using System.Security.Claims;
using System.Text.Json;
using IssueBoard.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace IssueBoard.Web.Auth;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal AnonymousUser = new(new ClaimsIdentity());

    private readonly ILocalStorageService _localStorageService;

    public JwtAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = await _localStorageService.GetItemAsync(
            AuthConstants.AccessTokenStorageKey);

        if (string.IsNullOrWhiteSpace(token) || IsTokenExpired(token))
        {
            await _localStorageService.RemoveItemAsync(
                AuthConstants.AccessTokenStorageKey);

            return new AuthenticationState(AnonymousUser);
        }

        ClaimsIdentity identity = new(
            ParseClaimsFromJwt(token),
            authenticationType: "jwt");

        ClaimsPrincipal user = new(identity);

        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticatedAsync(string accessToken)
    {
        await _localStorageService.SetItemAsync(
            AuthConstants.AccessTokenStorageKey,
            accessToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorageService.RemoveItemAsync(
            AuthConstants.AccessTokenStorageKey);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(AnonymousUser)));
    }

    private static bool IsTokenExpired(string jwt)
    {
        Dictionary<string, JsonElement>? claims = ReadJwtPayload(jwt);

        if (claims is null || !claims.TryGetValue("exp", out JsonElement expElement))
        {
            return true;
        }

        long exp = expElement.GetInt64();
        DateTimeOffset expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp);

        return expiresAt <= DateTimeOffset.UtcNow;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        Dictionary<string, JsonElement>? claims = ReadJwtPayload(jwt);

        if (claims is null)
        {
            yield break;
        }

        foreach ((string key, JsonElement value) in claims)
        {
            if (value.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in value.EnumerateArray())
                {
                    yield return new Claim(key, item.ToString());
                }

                continue;
            }

            yield return new Claim(key, value.ToString());
        }
    }

    private static Dictionary<string, JsonElement>? ReadJwtPayload(string jwt)
    {
        string[] parts = jwt.Split('.');

        if (parts.Length != 3)
        {
            return null;
        }

        string payload = parts[1];

        string base64 = payload
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        byte[] bytes = Convert.FromBase64String(base64);

        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(bytes);
    }
}
