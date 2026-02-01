# Pxshot .NET SDK

The official .NET SDK for the [Pxshot](https://pxshot.com) screenshot API.

[![NuGet](https://img.shields.io/nuget/v/Pxshot.svg)](https://www.nuget.org/packages/Pxshot)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Installation

```bash
dotnet add package Pxshot
```

## Quick Start

```csharp
using Pxshot;
using Pxshot.Models;

var client = new PxshotClient("px_your_api_key");

// Capture a screenshot as bytes
byte[] imageBytes = await client.ScreenshotAsync(new ScreenshotRequest
{
    Url = "https://example.com"
});

// Save to file
await File.WriteAllBytesAsync("screenshot.png", imageBytes);
```

## Features

- ✅ .NET 6, 7, and 8 support
- ✅ Async-first API
- ✅ Strongly-typed request/response models
- ✅ Rate limit header parsing
- ✅ Comprehensive exception handling
- ✅ Zero external dependencies (uses System.Text.Json)

## Usage Examples

### Basic Screenshot

```csharp
var client = new PxshotClient("px_your_api_key");

byte[] image = await client.ScreenshotAsync(new ScreenshotRequest
{
    Url = "https://example.com"
});
```

### Full Page Screenshot with Options

```csharp
byte[] image = await client.ScreenshotAsync(new ScreenshotRequest
{
    Url = "https://example.com",
    Format = "png",
    Width = 1920,
    Height = 1080,
    FullPage = true,
    DeviceScaleFactor = 2,
    WaitUntil = "networkidle",
    BlockAds = true
});
```

### Store Screenshot and Get URL

```csharp
var result = await client.ScreenshotWithStorageAsync(new ScreenshotRequest
{
    Url = "https://example.com"
});

Console.WriteLine($"Screenshot URL: {result.Url}");
Console.WriteLine($"Expires at: {result.ExpiresAt}");
Console.WriteLine($"Size: {result.SizeBytes} bytes");
```

### Wait for Specific Element

```csharp
byte[] image = await client.ScreenshotAsync(new ScreenshotRequest
{
    Url = "https://example.com",
    WaitForSelector = ".main-content",
    WaitForTimeout = 2000 // Additional 2 second wait
});
```

### Check Usage Statistics

```csharp
var usage = await client.GetUsageAsync();

Console.WriteLine($"Screenshots taken: {usage.ScreenshotsTaken}");
Console.WriteLine($"Screenshots remaining: {usage.ScreenshotsRemaining}");
Console.WriteLine($"Period ends: {usage.PeriodEnd}");
```

### Monitor Rate Limits

```csharp
var client = new PxshotClient("px_your_api_key");

await client.ScreenshotAsync(new ScreenshotRequest { Url = "https://example.com" });

if (client.LastRateLimitInfo is { } rateLimit)
{
    Console.WriteLine($"Rate limit: {rateLimit.Remaining}/{rateLimit.Limit}");
    Console.WriteLine($"Resets at: {rateLimit.Reset}");
}
```

### Using Custom HttpClient

```csharp
// Useful for dependency injection or custom configurations
var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromMinutes(2);

var client = new PxshotClient("px_your_api_key", null, httpClient);
```

### Custom Base URL

```csharp
// For self-hosted or staging environments
var client = new PxshotClient("px_your_api_key", "https://staging-api.pxshot.com");
```

## Exception Handling

```csharp
using Pxshot.Exceptions;

try
{
    var image = await client.ScreenshotAsync(new ScreenshotRequest
    {
        Url = "https://example.com"
    });
}
catch (RateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
    if (ex.RateLimitInfo is { } info)
    {
        Console.WriteLine($"Limit: {info.Limit}, Remaining: {info.Remaining}");
    }
}
catch (AuthenticationException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Invalid request: {ex.Message}");
}
catch (PxshotException ex)
{
    Console.WriteLine($"API error ({ex.StatusCode}): {ex.Message}");
}
```

## Request Options

| Option | Type | Description |
|--------|------|-------------|
| `Url` | `string` | **Required.** The URL to capture. |
| `Format` | `string` | Image format: `"png"`, `"jpeg"`, `"webp"`. Default: `"png"` |
| `Quality` | `int` | Image quality for JPEG/WebP (1-100). Default: `80` |
| `Width` | `int` | Viewport width in pixels. Default: `1280` |
| `Height` | `int` | Viewport height in pixels. Default: `720` |
| `FullPage` | `bool` | Capture full scrollable page. Default: `false` |
| `WaitUntil` | `string` | Wait condition: `"load"`, `"domcontentloaded"`, `"networkidle"` |
| `WaitForSelector` | `string` | CSS selector to wait for before capturing |
| `WaitForTimeout` | `int` | Additional wait time in milliseconds |
| `DeviceScaleFactor` | `double` | Device pixel ratio. Default: `1` |
| `Store` | `bool` | Store screenshot and return URL. Default: `false` |

## License

MIT License - see [LICENSE](LICENSE) for details.
