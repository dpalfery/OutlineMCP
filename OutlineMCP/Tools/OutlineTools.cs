using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using ModelContextProtocol.Server;
using OutlineMCP.Settings;

namespace OutlineMCP.Services;

[McpServerToolType]
public class OutlineTools
{
    private readonly HttpClient _httpClient;
    private readonly OutlineSettings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    // Security constants
    private const int MaxQueryLength = 1000;
    private const int MaxDocumentIdLength = 255;
    private const int MaxLimitValue = 100;
    private static readonly Regex ValidDocumentIdPattern = new(@"^[a-zA-Z0-9\-_]{1,255}$", RegexOptions.Compiled);
    private static readonly Regex SafeQueryPattern = new(@"^[^<>""';&|`$]*$", RegexOptions.Compiled);

    public OutlineTools(HttpClient httpClient, OutlineSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    /// <summary>
    /// Validates and sanitizes input parameters to prevent injection attacks
    /// </summary>
    private static string ValidateAndSanitizeQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be null or empty", nameof(query));
        
        if (query.Length > MaxQueryLength)
            throw new ArgumentException($"Query length cannot exceed {MaxQueryLength} characters", nameof(query));
        
        var trimmedQuery = query.Trim();
        
        if (!SafeQueryPattern.IsMatch(trimmedQuery))
            throw new ArgumentException("Query contains potentially unsafe characters", nameof(query));
        
        return HttpUtility.HtmlEncode(trimmedQuery);
    }

    /// <summary>
    /// Validates document ID format
    /// </summary>
    private static string ValidateDocumentId(string documentId)
    {
        if (string.IsNullOrWhiteSpace(documentId))
            throw new ArgumentException("Document ID cannot be null or empty", nameof(documentId));
        
        if (documentId.Length > MaxDocumentIdLength)
            throw new ArgumentException($"Document ID length cannot exceed {MaxDocumentIdLength} characters", nameof(documentId));
        
        if (!ValidDocumentIdPattern.IsMatch(documentId))
            throw new ArgumentException("Document ID contains invalid characters", nameof(documentId));
        
        return documentId;
    }

    /// <summary>
    /// Validates limit parameter
    /// </summary>
    private static int ValidateLimit(int limit)
    {
        if (limit <= 0)
            throw new ArgumentException("Limit must be greater than 0", nameof(limit));
        
        if (limit > MaxLimitValue)
            throw new ArgumentException($"Limit cannot exceed {MaxLimitValue}", nameof(limit));
        
        return limit;
    }

    /// <summary>
    /// Safely constructs API URL
    /// </summary>
    private static string ConstructApiUrl(string baseUrl, string endpoint)
    {
        var normalizedBaseUrl = baseUrl.TrimEnd('/');
        if (!normalizedBaseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Base URL must use HTTPS protocol for security");
        }
        return $"{normalizedBaseUrl}/api/{endpoint}";
    }

    /// <summary>
    /// Creates a secure HTTP request with proper authorization
    /// </summary>
    private HttpRequestMessage CreateSecureRequest(HttpMethod method, string url, object? data = null)
    {
        var request = new HttpRequestMessage(method, url);
        
        if (!string.IsNullOrEmpty(_settings.ApiToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiToken);
        }
        
        if (data != null)
        {
            request.Content = JsonContent.Create(data, options: JsonOptions);
        }
        
        return request;
    }

    /// <summary>
    /// Validates configuration and returns sanitized error messages
    /// </summary>
    private string? ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
        {
            return "Configuration error: Base URL is not configured. Please set OUTLINE_BASE_URL environment variable.";
        }
        
        if (string.IsNullOrWhiteSpace(_settings.ApiToken))
        {
            return "Configuration error: API token is not configured. Please set OUTLINE_API_TOKEN environment variable.";
        }
        
        try
        {
            var uri = new Uri(_settings.BaseUrl);
            if (uri.Scheme != "https")
            {
                return "Configuration error: Base URL must use HTTPS protocol.";
            }
        }
        catch (UriFormatException)
        {
            return "Configuration error: Base URL format is invalid.";
        }
        
        return null;
    }

    [McpServerTool, Description("Search for documents in the Outline documentation system")]
    public async Task<string> SearchDocuments(string query, int limit = 20)
    {
        try
        {
            // Validate configuration
            var configError = ValidateConfiguration();
            if (configError != null)
                return configError;

            // Validate and sanitize inputs
            var sanitizedQuery = ValidateAndSanitizeQuery(query);
            var validatedLimit = ValidateLimit(limit);
            
            var searchUrl = ConstructApiUrl(_settings.BaseUrl!, "documents.search");
            var searchData = new { query = sanitizedQuery, limit = validatedLimit };
            
            using var request = CreateSecureRequest(HttpMethod.Post, searchUrl, searchData);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Search results for query: {result}";
        }
        catch (ArgumentException ex)
        {
            return $"Invalid input: {ex.Message}";
        }
        catch (HttpRequestException)
        {
            return "Error: Unable to connect to Outline API. Please check your network connection and API configuration.";
        }
        catch (Exception)
        {
            return "Error: An unexpected error occurred while searching documents.";
        }
    }

    [McpServerTool, Description("Search a Wiki for relevant information")]
    public async Task<string> SearchWiki(string query, int limit = 20)
    {
        try
        {
            // Validate configuration
            var configError = ValidateConfiguration();
            if (configError != null)
                return configError;

            // Validate and sanitize inputs
            var sanitizedQuery = ValidateAndSanitizeQuery(query);
            var validatedLimit = ValidateLimit(limit);
            
            var searchUrl = ConstructApiUrl(_settings.BaseUrl!, "documents.search");
            var searchData = new { query = sanitizedQuery, limit = validatedLimit };
            
            using var request = CreateSecureRequest(HttpMethod.Post, searchUrl, searchData);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Wiki search results: {result}";
        }
        catch (ArgumentException ex)
        {
            return $"Invalid input: {ex.Message}";
        }
        catch (HttpRequestException)
        {
            return "Error: Unable to connect to Outline API. Please check your network connection and API configuration.";
        }
        catch (Exception)
        {
            return "Error: An unexpected error occurred while searching wiki.";
        }
    }

    [McpServerTool, Description("Get a specific document by ID")]
    public async Task<string> GetDocument(string documentId)
    {
        try
        {
            // Validate configuration
            var configError = ValidateConfiguration();
            if (configError != null)
                return configError;

            // Validate document ID
            var validatedDocumentId = ValidateDocumentId(documentId);
            
            var documentUrl = ConstructApiUrl(_settings.BaseUrl!, "documents.info");
            var documentData = new { id = validatedDocumentId };
            
            using var request = CreateSecureRequest(HttpMethod.Post, documentUrl, documentData);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Document: {result}";
        }
        catch (ArgumentException ex)
        {
            return $"Invalid input: {ex.Message}";
        }
        catch (HttpRequestException)
        {
            return "Error: Unable to connect to Outline API. Please check your network connection and API configuration.";
        }
        catch (Exception)
        {
            return "Error: An unexpected error occurred while retrieving document.";
        }
    }

    [McpServerTool, Description("List documents with optional filtering")]
    public async Task<string> ListDocuments(int limit = 20, string? sort = null, bool? starred = null)
    {
        try
        {
            // Validate configuration
            var configError = ValidateConfiguration();
            if (configError != null)
                return configError;

            // Validate inputs
            var validatedLimit = ValidateLimit(limit);
            
            // Validate sort parameter if provided
            if (!string.IsNullOrEmpty(sort))
            {
                var allowedSortValues = new[] { "createdAt", "updatedAt", "title", "index" };
                if (!allowedSortValues.Contains(sort, StringComparer.OrdinalIgnoreCase))
                {
                    return "Invalid input: Sort parameter must be one of: createdAt, updatedAt, title, index";
                }
            }
            
            var documentsUrl = ConstructApiUrl(_settings.BaseUrl!, "documents.list");
            var documentsData = new { limit = validatedLimit, sort, starred };
            
            using var request = CreateSecureRequest(HttpMethod.Post, documentsUrl, documentsData);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Documents list: {result}";
        }
        catch (ArgumentException ex)
        {
            return $"Invalid input: {ex.Message}";
        }
        catch (HttpRequestException)
        {
            return "Error: Unable to connect to Outline API. Please check your network connection and API configuration.";
        }
        catch (Exception)
        {
            return "Error: An unexpected error occurred while listing documents.";
        }
    }

    [McpServerTool, Description("List all available collections")]
    public async Task<string> ListCollections(int limit = 20)
    {
        try
        {
            // Validate configuration
            var configError = ValidateConfiguration();
            if (configError != null)
                return configError;

            // Validate inputs
            var validatedLimit = ValidateLimit(limit);
            
            var collectionsUrl = ConstructApiUrl(_settings.BaseUrl!, "collections.list");
            var collectionsData = new { limit = validatedLimit };
            
            using var request = CreateSecureRequest(HttpMethod.Post, collectionsUrl, collectionsData);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Collections list: {result}";
        }
        catch (ArgumentException ex)
        {
            return $"Invalid input: {ex.Message}";
        }
        catch (HttpRequestException)
        {
            return "Error: Unable to connect to Outline API. Please check your network connection and API configuration.";
        }
        catch (Exception)
        {
            return "Error: An unexpected error occurred while listing collections.";
        }
    }
}