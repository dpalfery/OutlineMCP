using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    public OutlineTools(HttpClient httpClient, OutlineSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    [McpServerTool, Description("Search for documents in the Outline documentation system")]
    public async Task<string> SearchDocuments(string query, int limit = 20)
    {
        try
        {
            var baseUrl = _settings.BaseUrl;
            var apiToken = _settings.ApiToken;
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                return "Error: Base URL is not set. Please configure OUTLINE_BASE_URL environment variable or set it in the configuration.";
            }
            
            if (string.IsNullOrEmpty(apiToken))
            {
                return "Error: API token is not set. Please configure OUTLINE_API_TOKEN environment variable or set it in the configuration.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            
            var searchUrl = $"{baseUrl}api/documents.search";
            var searchData = new { query, limit };
            
            var response = await _httpClient.PostAsJsonAsync(searchUrl, searchData, JsonOptions);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Search results for '{query}': {result}";
        }
        catch (Exception ex)
        {
            return $"Error searching documents: {ex.Message}";
        }
    }

    [McpServerTool, Description("Search a Wiki for relevant information")]
    public async Task<string> SearchWiki(string query, int limit = 20)
    {
        try
        {
            var baseUrl = _settings.BaseUrl;
            var apiToken = _settings.ApiToken;
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                return "Error: Base URL is not set. Please configure OUTLINE_BASE_URL environment variable or set it in the configuration.";
            }
            
            if (string.IsNullOrEmpty(apiToken))
            {
                return "Error: API token is not set. Please configure OUTLINE_API_TOKEN environment variable or set it in the configuration.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            
            var searchUrl = $"{baseUrl}api/documents.search";
            var searchData = new { query, limit };
            
            var response = await _httpClient.PostAsJsonAsync(searchUrl, searchData, JsonOptions);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Wiki search results for '{query}': {result}";
        }
        catch (Exception ex)
        {
            return $"Error searching wiki: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get a specific document by ID")]
    public async Task<string> GetDocument(string documentId)
    {
        try
        {
            var baseUrl = _settings.BaseUrl;
            var apiToken = _settings.ApiToken;
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                return "Error: Base URL is not set. Please configure OUTLINE_BASE_URL environment variable or set it in the configuration.";
            }
            
            if (string.IsNullOrEmpty(apiToken))
            {
                return "Error: API token is not set. Please configure OUTLINE_API_TOKEN environment variable or set it in the configuration.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            
            var documentUrl = $"{baseUrl}api/documents.info";
            var documentData = new { id = documentId };
            
            var response = await _httpClient.PostAsJsonAsync(documentUrl, documentData, JsonOptions);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Document {documentId}: {result}";
        }
        catch (Exception ex)
        {
            return $"Error getting document: {ex.Message}";
        }
    }

    [McpServerTool, Description("List documents with optional filtering")]
    public async Task<string> ListDocuments(int limit = 20, string? sort = null, bool? starred = null)
    {
        try
        {
            var baseUrl = _settings.BaseUrl;
            var apiToken = _settings.ApiToken;
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                return "Error: Base URL is not set. Please configure OUTLINE_BASE_URL environment variable or set it in the configuration.";
            }
            
            if (string.IsNullOrEmpty(apiToken))
            {
                return "Error: API token is not set. Please configure OUTLINE_API_TOKEN environment variable or set it in the configuration.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            
            var documentsUrl = $"{baseUrl}api/documents.list";
            var documentsData = new { limit, sort, starred };
            
            var response = await _httpClient.PostAsJsonAsync(documentsUrl, documentsData, JsonOptions);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Documents list: {result}";
        }
        catch (Exception ex)
        {
            return $"Error listing documents: {ex.Message}";
        }
    }

    [McpServerTool, Description("List all available collections")]
    public async Task<string> ListCollections(int limit = 20)
    {
        try
        {
            var baseUrl = _settings.BaseUrl;
            var apiToken = _settings.ApiToken;
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                return "Error: Base URL is not set. Please configure OUTLINE_BASE_URL environment variable or set it in the configuration.";
            }
            
            if (string.IsNullOrEmpty(apiToken))
            {
                return "Error: API token is not set. Please configure OUTLINE_API_TOKEN environment variable or set it in the configuration.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            
            var collectionsUrl = $"{baseUrl}api/collections.list";
            var collectionsData = new { limit };
            
            var response = await _httpClient.PostAsJsonAsync(collectionsUrl, collectionsData, JsonOptions);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            return $"Collections list: {result}";
        }
        catch (Exception ex)
        {
            return $"Error listing collections: {ex.Message}";
        }
    }
}