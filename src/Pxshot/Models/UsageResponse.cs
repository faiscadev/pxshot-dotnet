namespace Pxshot.Models;

/// <summary>
/// Response containing usage statistics for your account.
/// </summary>
public record UsageResponse
{
    /// <summary>
    /// Total number of screenshots captured this billing period.
    /// </summary>
    public int ScreenshotsTaken { get; init; }

    /// <summary>
    /// Maximum number of screenshots allowed per billing period.
    /// </summary>
    public int ScreenshotsLimit { get; init; }

    /// <summary>
    /// Number of screenshots remaining in this billing period.
    /// </summary>
    public int ScreenshotsRemaining { get; init; }

    /// <summary>
    /// Start of the current billing period.
    /// </summary>
    public DateTimeOffset? PeriodStart { get; init; }

    /// <summary>
    /// End of the current billing period.
    /// </summary>
    public DateTimeOffset? PeriodEnd { get; init; }
}
