using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;
using OutlineMCP.Settings;

namespace OutlineMCP.Services;

/// <summary>
/// Resources for the Outline documentation system
/// </summary>
[McpServerResourceType]
public class OutlineResources
{
    private readonly OutlineSettings _settings;

    public OutlineResources(OutlineSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Provides information about the Outline API endpoints
    /// </summary>
    [McpServerResource, Description("Information about available Outline API endpoints")]
    public string GetApiEndpoints()
    {
        return @"
# Outline API Endpoints

The following API endpoints are available in the Outline documentation system:

## Documents
- `/api/documents.list` - List all documents
- `/api/documents.search` - Search for documents
- `/api/documents.info` - Get document details

## Wiki
- `/api/wiki.search` - Search wiki content

## Collections
- `/api/collections.list` - List all collections
- `/api/collections.info` - Get collection details

For more information, refer to the official Outline API documentation.
";
    }

    /// <summary>
    /// Provides instructions for using the Outline tools
    /// </summary>
    [McpServerResource, Description("Instructions for using Outline tools")]
    public string GetUsageInstructions()
    {
        return @"
# Using Outline Documentation Tools

## Search Capabilities
- Use `SearchDocuments` for general document search across all collections
- Use `SearchWiki` to specifically search wiki content
- Use collection-specific search methods for targeted searches

## Viewing and Browsing Content
- Use `GetDocument` with a document ID to retrieve specific document content
- Use `ListDocuments` to browse available documents
- Use `ListCollections` to see all available document collections

## Tips for Effective Searching
1. Be specific with search terms
2. Use collection-specific searches when you know where the content should be
3. Limit results by setting the `limit` parameter (default is 20)
";
    }

    /// <summary>
    /// Returns information about the configured Outline instance
    /// </summary>
    [McpServerResource, Description("Information about the configured Outline instance")]
    public string GetOutlineInfo()
    {
        var baseUrl = _settings.BaseUrl ?? "Not configured";
        var hasApiToken = !string.IsNullOrEmpty(_settings.ApiToken);
        
        var sb = new StringBuilder();
        sb.AppendLine("# Outline Configuration Information");
        sb.AppendLine();
        sb.AppendLine($"Base URL: {baseUrl}");
        sb.AppendLine($"API Token Configured: {(hasApiToken ? "Yes" : "No")}");
        sb.AppendLine();
        sb.AppendLine("## Environment Setup");
        sb.AppendLine("The Outline API requires proper configuration to function correctly:");
        sb.AppendLine("1. `OUTLINE_BASE_URL` environment variable should point to your Outline instance");
        sb.AppendLine("2. `OUTLINE_API_TOKEN` environment variable must contain a valid API token");
        
        return sb.ToString();
    }

    /// <summary>
    /// Provides guidance on troubleshooting common Outline API issues
    /// </summary>
    [McpServerResource, Description("Troubleshooting guide for common Outline API issues")]
    public string GetTroubleshootingGuide()
    {
        return @"
# Troubleshooting Outline API

## Common Issues and Solutions

### Authentication Errors
- Verify that your API token is correct and not expired
- Check that the API token has appropriate permissions
- Ensure the token is being sent in the Authorization header

### Connection Issues
- Verify the base URL is correct and includes the protocol (https://)
- Check network connectivity to the Outline server
- Verify that the Outline server is operational

### Search Problems
- Ensure search terms are not too generic or too specific
- Check collection IDs if searching specific collections
- Verify document permissions if certain content is not appearing in results

### Performance Issues
- Consider reducing the limit parameter for faster responses
- Use more specific search terms to narrow results
- If possible, search within specific collections rather than all documents

Contact your Outline administrator if issues persist after trying these solutions.
";
    }
}