using System;

namespace OutlineMCP.Settings;

/// <summary>
/// Settings for the Outline API
/// </summary>
public class OutlineSettings
{
    /// <summary>
    /// The base URL for the Outline API
    /// </summary>
    public string? BaseUrl { get; set; } = Environment.GetEnvironmentVariable("OUTLINE_BASE_URL");

    /// <summary>
    /// The API token for authenticating with the Outline API
    /// </summary>
    public string? ApiToken { get; set; } = Environment.GetEnvironmentVariable("OUTLINE_API_TOKEN");
}