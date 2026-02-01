namespace Pxshot.Models;

/// <summary>
/// Rate limit information from the API response headers.
/// </summary>
public record RateLimitInfo
{
    /// <summary>
    /// Maximum number of requests allowed per window.
    /// </summary>
    public int? Limit { get; }

    /// <summary>
    /// Number of requests remaining in the current window.
    /// </summary>
    public int? Remaining { get; }

    /// <summary>
    /// When the rate limit window resets.
    /// </summary>
    public DateTimeOffset? Reset { get; }

    /// <summary>
    /// Creates a new RateLimitInfo instance.
    /// </summary>
    public RateLimitInfo(int? limit, int? remaining, DateTimeOffset? reset)
    {
        Limit = limit;
        Remaining = remaining;
        Reset = reset;
    }

    /// <summary>
    /// Gets the time until the rate limit resets, or null if unknown.
    /// </summary>
    public TimeSpan? TimeUntilReset => Reset.HasValue
        ? Reset.Value - DateTimeOffset.UtcNow
        : null;
}
