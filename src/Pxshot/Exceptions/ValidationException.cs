namespace Pxshot.Exceptions;

/// <summary>
/// Exception thrown when request validation fails.
/// </summary>
public class ValidationException : PxshotException
{
    /// <summary>
    /// Creates a new ValidationException.
    /// </summary>
    public ValidationException(string message, int statusCode, string? errorCode = null)
        : base(message, statusCode, errorCode)
    {
    }
}
