namespace Pxshot.Exceptions;

/// <summary>
/// Base exception for all Pxshot API errors.
/// </summary>
public class PxshotException : Exception
{
    /// <summary>
    /// HTTP status code from the API response.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Error code from the API response.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Creates a new PxshotException.
    /// </summary>
    public PxshotException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new PxshotException with status code and error code.
    /// </summary>
    public PxshotException(string message, int statusCode, string? errorCode = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a new PxshotException with an inner exception.
    /// </summary>
    public PxshotException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
