using System.Net;

namespace YoutubeChannelAPI.Models;

/// <summary>
/// Resulting object if an API call fails
/// </summary>
public class ErrorResult
{
    /// <summary>
    /// List of error messages
    /// </summary>
    public List<string> Messages { get; set; } = new();

    /// <summary>
    /// Path to the file where the error occurred
    /// </summary>
    public string? Source { get; set; }
    
    /// <summary>
    /// Message of the exception that was thrown
    /// </summary>
    public string? Exception { get; init; }
    
    /// <summary>
    /// UUID of this error
    /// </summary>
    public string? ErrorId { get; set; }
    
    /// <summary>
    /// Message to contact the support team
    /// </summary>
    public string? SupportMessage { get; set; }
    
    /// <summary>
    /// Status code of the request
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }
}