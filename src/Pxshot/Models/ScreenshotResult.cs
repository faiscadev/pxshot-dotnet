namespace Pxshot.Models;

/// <summary>
/// Result of a stored screenshot request.
/// </summary>
public record ScreenshotResult
{
    /// <summary>
    /// URL where the screenshot is stored.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// When the stored screenshot expires.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Width of the captured screenshot in pixels.
    /// </summary>
    public int? Width { get; init; }

    /// <summary>
    /// Height of the captured screenshot in pixels.
    /// </summary>
    public int? Height { get; init; }

    /// <summary>
    /// Size of the screenshot in bytes.
    /// </summary>
    public long? SizeBytes { get; init; }
}
