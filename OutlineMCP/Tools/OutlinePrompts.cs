using System.ComponentModel;
using ModelContextProtocol.Server;

namespace OutlineMCP.Services;

[McpServerPromptType]
public static class OutlinePrompts
{
    [McpServerPrompt, Description("Creates a general documentation search query")]
    public static string GeneralSearch(string query)
    {
        return $@"
Search the Outline documentation for information on the following topic: {query}

Consider:
- Technical documentation
- Product specifications
- User guides
- Troubleshooting articles

Return the most relevant information found.";
    }

   

    [McpServerPrompt, Description("Creates a query to find technical specifications")]
    public static string TechnicalSpecifications(string query)
    {
        return $@"
Search the Outline documentation for technical specifications related to: {query}

Include:
- System requirements
- Architecture details
- Performance metrics
- Scalability information
- Technical limitations
- Compatibility information

Return specific technical details and specifications.";
    }

    [McpServerPrompt, Description("Creates a query to find troubleshooting information")]
    public static string TroubleshootingGuide(string query)
    {
        return $@"
Search the Outline documentation for troubleshooting information about: {query}

Include:
- Common error messages
- Problem diagnosis steps
- Resolution procedures
- Known issues
- Workarounds
- Support contact information

Return step-by-step troubleshooting guidance.";
    }
}