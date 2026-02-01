namespace Pxshot.Models;

/// <summary>
/// Request parameters for capturing a screenshot.
/// </summary>
public record ScreenshotRequest
{
    /// <summary>
    /// The URL to capture. Required.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Output image format. Defaults to "png".
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// Image quality for JPEG/WebP (1-100). Defaults to 80.
    /// </summary>
    public int? Quality { get; init; }

    /// <summary>
    /// Viewport width in pixels. Defaults to 1280.
    /// </summary>
    public int? Width { get; init; }

    /// <summary>
    /// Viewport height in pixels. Defaults to 720.
    /// </summary>
    public int? Height { get; init; }

    /// <summary>
    /// Whether to capture the full scrollable page. Defaults to false.
    /// </summary>
    public bool? FullPage { get; init; }

    /// <summary>
    /// Wait condition before capturing. Options: "load", "domcontentloaded", "networkidle".
    /// </summary>
    public string? WaitUntil { get; init; }

    /// <summary>
    /// CSS selector to wait for before capturing.
    /// </summary>
    public string? WaitForSelector { get; init; }

    /// <summary>
    /// Additional time to wait in milliseconds after the page loads.
    /// </summary>
    public int? WaitForTimeout { get; init; }

    /// <summary>
    /// Device scale factor (DPR). Defaults to 1.
    /// </summary>
    public double? DeviceScaleFactor { get; init; }

    /// <summary>
    /// Whether to store the screenshot and return a URL instead of bytes.
    /// When true, returns a ScreenshotResult with URL. When false, returns raw bytes.
    /// </summary>
    public bool? Store { get; init; }

    /// <summary>
    /// Whether to block ads and trackers. Defaults to false.
    /// </summary>
    public bool? BlockAds { get; init; }
}
