using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Pxshot.Exceptions;
using Pxshot.Models;

namespace Pxshot;

/// <summary>
/// Client for interacting with the Pxshot screenshot API.
/// </summary>
public sealed class PxshotClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// The default base URL for the Pxshot API.
    /// </summary>
    public const string DefaultBaseUrl = "https://api.pxshot.com";

    /// <summary>
    /// Gets the rate limit information from the last API response.
    /// </summary>
    public RateLimitInfo? LastRateLimitInfo { get; private set; }

    /// <summary>
    /// Creates a new PxshotClient with the specified API key.
    /// </summary>
    /// <param name="apiKey">Your Pxshot API key.</param>
    /// <param name="baseUrl">Optional custom base URL for the API.</param>
    public PxshotClient(string apiKey, string? baseUrl = null)
        : this(apiKey, baseUrl, null)
    {
    }

    /// <summary>
    /// Creates a new PxshotClient with the specified API key and HttpClient.
    /// </summary>
    /// <param name="apiKey">Your Pxshot API key.</param>
    /// <param name="baseUrl">Optional custom base URL for the API.</param>
    /// <param name="httpClient">Optional HttpClient to use for requests. If provided, it will not be disposed by PxshotClient.</param>
    public PxshotClient(string apiKey, string? baseUrl, HttpClient? httpClient)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));

        if (httpClient is null)
        {
            _httpClient = new HttpClient();
            _disposeHttpClient = true;
        }
        else
        {
            _httpClient = httpClient;
            _disposeHttpClient = false;
        }

        _httpClient.BaseAddress = new Uri(baseUrl ?? DefaultBaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Pxshot-DotNet/1.0.0");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Captures a screenshot and returns the image bytes.
    /// </summary>
    /// <param name="request">The screenshot request parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The screenshot image as a byte array.</returns>
    /// <exception cref="PxshotException">Thrown when the API returns an error.</exception>
    /// <exception cref="RateLimitException">Thrown when the rate limit is exceeded.</exception>
    public async Task<byte[]> ScreenshotAsync(ScreenshotRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Store == true)
            throw new ArgumentException("Use ScreenshotWithStorageAsync when Store is true.", nameof(request));

        var response = await SendScreenshotRequestAsync(request, cancellationToken);
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Captures a screenshot and stores it, returning metadata including the URL.
    /// </summary>
    /// <param name="request">The screenshot request parameters. Store will be set to true.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The screenshot result with URL and metadata.</returns>
    /// <exception cref="PxshotException">Thrown when the API returns an error.</exception>
    /// <exception cref="RateLimitException">Thrown when the rate limit is exceeded.</exception>
    public async Task<ScreenshotResult> ScreenshotWithStorageAsync(ScreenshotRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Ensure store is true for this method
        var storageRequest = request with { Store = true };

        var response = await SendScreenshotRequestAsync(storageRequest, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ScreenshotResult>(_jsonOptions, cancellationToken);

        return result ?? throw new PxshotException("Failed to deserialize screenshot result.");
    }

    /// <summary>
    /// Gets the current usage statistics for your account.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Usage statistics.</returns>
    /// <exception cref="PxshotException">Thrown when the API returns an error.</exception>
    /// <exception cref="RateLimitException">Thrown when the rate limit is exceeded.</exception>
    public async Task<UsageResponse> GetUsageAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/v1/usage", cancellationToken);
        UpdateRateLimitInfo(response);
        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<UsageResponse>(_jsonOptions, cancellationToken);
        return result ?? throw new PxshotException("Failed to deserialize usage response.");
    }

    private async Task<HttpResponseMessage> SendScreenshotRequestAsync(ScreenshotRequest request, CancellationToken cancellationToken)
    {
        var content = JsonContent.Create(request, options: _jsonOptions);
        var response = await _httpClient.PostAsync("/v1/screenshot", content, cancellationToken);

        UpdateRateLimitInfo(response);
        await EnsureSuccessAsync(response, cancellationToken);

        return response;
    }

    private void UpdateRateLimitInfo(HttpResponseMessage response)
    {
        var headers = response.Headers;

        int? limit = null;
        int? remaining = null;
        DateTimeOffset? reset = null;

        if (headers.TryGetValues("X-RateLimit-Limit", out var limitValues) &&
            int.TryParse(limitValues.FirstOrDefault(), out var limitValue))
        {
            limit = limitValue;
        }

        if (headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues) &&
            int.TryParse(remainingValues.FirstOrDefault(), out var remainingValue))
        {
            remaining = remainingValue;
        }

        if (headers.TryGetValues("X-RateLimit-Reset", out var resetValues) &&
            long.TryParse(resetValues.FirstOrDefault(), out var resetTimestamp))
        {
            reset = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp);
        }

        if (limit.HasValue || remaining.HasValue || reset.HasValue)
        {
            LastRateLimitInfo = new RateLimitInfo(limit, remaining, reset);
        }
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var statusCode = (int)response.StatusCode;
        string? errorMessage = null;
        string? errorCode = null;

        try
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(_jsonOptions, cancellationToken);
            errorMessage = errorResponse?.Error?.Message ?? errorResponse?.Message;
            errorCode = errorResponse?.Error?.Code ?? errorResponse?.Code;
        }
        catch
        {
            // If we can't parse the error response, we'll use a generic message
        }

        errorMessage ??= $"API request failed with status code {statusCode}";

        if (statusCode == 429)
        {
            var retryAfter = GetRetryAfter(response);
            throw new RateLimitException(errorMessage, statusCode, errorCode, retryAfter, LastRateLimitInfo);
        }

        if (statusCode == 401)
        {
            throw new AuthenticationException(errorMessage, statusCode, errorCode);
        }

        if (statusCode == 400)
        {
            throw new ValidationException(errorMessage, statusCode, errorCode);
        }

        throw new PxshotException(errorMessage, statusCode, errorCode);
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Retry-After", out var values))
        {
            var value = values.FirstOrDefault();
            if (int.TryParse(value, out var seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }
        }
        return null;
    }

    /// <summary>
    /// Disposes the HttpClient if it was created by this instance.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private record ErrorResponse(ErrorDetail? Error, string? Message, string? Code);
    private record ErrorDetail(string? Message, string? Code);
}
