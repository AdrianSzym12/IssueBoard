using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IssueBoard.Web.Auth;

namespace IssueBoard.Web.Services;

public sealed class ApiClient : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public ApiClient(
        HttpClient httpClient,
        ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    public async Task<ApiResult<TResponse>> GetAsync<TResponse>(
        string url,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, url);

        await AddAuthorizationHeaderAsync(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        return await ReadResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest requestBody,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(requestBody, options: JsonOptions)
        };

        await AddAuthorizationHeaderAsync(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        return await ReadResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
        string url,
        TRequest requestBody,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(requestBody, options: JsonOptions)
        };

        await AddAuthorizationHeaderAsync(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        return await ReadResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<ApiResult<bool>> DeleteAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Delete, url);

        await AddAuthorizationHeaderAsync(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return ApiResult<bool>.Success(true);
        }

        string error = await ReadErrorAsync(response, cancellationToken);

        return ApiResult<bool>.Failure(
            error,
            (int)response.StatusCode);
    }

    private async Task AddAuthorizationHeaderAsync(HttpRequestMessage request)
    {
        string? accessToken = await _localStorageService.GetItemAsync(
            AuthConstants.AccessTokenStorageKey);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken);
        }
    }

    private static async Task<ApiResult<TResponse>> ReadResponseAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            string error = await ReadErrorAsync(response, cancellationToken);

            return ApiResult<TResponse>.Failure(
                error,
                (int)response.StatusCode);
        }

        TResponse? value = await response.Content.ReadFromJsonAsync<TResponse>(
            JsonOptions,
            cancellationToken);

        if (value is null)
        {
            return ApiResult<TResponse>.Failure("Empty response from API.");
        }

        return ApiResult<TResponse>.Success(value);
    }

    private static async Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        string raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(raw))
        {
            return $"API request failed with status code {(int)response.StatusCode}.";
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(raw);

            if (document.RootElement.TryGetProperty("message", out JsonElement message))
            {
                return message.GetString()
                    ?? $"API request failed with status code {(int)response.StatusCode}.";
            }

            if (document.RootElement.TryGetProperty("title", out JsonElement title))
            {
                return title.GetString()
                    ?? $"API request failed with status code {(int)response.StatusCode}.";
            }

            return raw;
        }
        catch (JsonException)
        {
            return raw;
        }
    }
}
