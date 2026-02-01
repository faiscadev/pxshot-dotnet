namespace Pxshot.Exceptions;

/// <summary>
/// Exception thrown when authentication fails (invalid or missing API key).
/// </summary>
public class AuthenticationException : PxshotException
{
    /// <summary>
    /// Creates a new AuthenticationException.
    /// </summary>
    public AuthenticationException(string message, int statusCode, string? errorCode = null)
        : base(message, statusCode, errorCode)
    {
    }
}
