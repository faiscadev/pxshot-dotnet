using Pxshot.Models;

namespace Pxshot.Exceptions;

/// <summary>
/// Exception thrown when the API rate limit is exceeded.
/// </summary>
public class RateLimitException : PxshotException
{
    /// <summary>
    /// How long to wait before retrying, if provided by the API.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>
    /// Rate limit information from the response headers.
    /// </summary>
    public RateLimitInfo? RateLimitInfo { get; }

    /// <summary>
    /// Creates a new RateLimitException.
    /// </summary>
    public RateLimitException(string message, int statusCode, string? errorCode, TimeSpan? retryAfter, RateLimitInfo? rateLimitInfo)
        : base(message, statusCode, errorCode)
    {
        RetryAfter = retryAfter;
        RateLimitInfo = rateLimitInfo;
    }
}
