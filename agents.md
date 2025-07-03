# OutlineMCP Agent Documentation

This repository contains an MCP (Model Context Protocol) server implementation for Outline documentation system. The server provides a bridge between Cursor's AI capabilities and Outline's documentation API.

## Project Structure

```
OutlineMCP/
├── OutlineMCP/              # Main MCP server implementation
│   ├── Services/           # Service implementations
│   ├── Models/            # Data models and DTOs
│   ├── Program.cs         # Server entry point and configuration
│   ├── OutlineMCP.csproj  # Project file with dependencies
│   └── appsettings.json   # Configuration settings
└── OutlineMCP.Tests/       # Test project
    └── OutlineServiceTests.cs  # Service unit tests
```
## Developer instructions
do not place multiple classes in the same file
Coding Style
Use Microsoft C# Coding Conventions.
Indentation: 4 spaces.
File Structure:
Keep each class in its own file.
Group related files by feature or domain in folders.
Naming:
Classes & interfaces: PascalCase (OrderService, ICustomerRepository)
Methods & properties: PascalCase
Parameters & local variables: camelCase
Constants: PascalCase or UPPER_SNAKE_CASE
Namespaces: Follow folder structure and project organization.
Async: Use async/await throughout; name async methods with Async suffix.
Nullable: Use nullable reference types (#nullable enable).
Clean Architecture Rules
No project may reference a project "up" the architecture:
Domain has no dependencies.
Application depends only on Domain.
Infrastructure depends on Application and Domain.
API depends on Application, Infrastructure, and Domain.
Business logic goes only in Application.
Infrastructure contains EF Core, external APIs, and implementation details.
API contains only presentation and configuration logic.
Use dependency injection for all external services and infrastructure dependencies.
Testing
All business logic must be covered by unit tests (xUnit recommended).
Integration tests should cover key application and infrastructure boundaries.
Mock dependencies in application tests.
Use FluentAssertions for assertions.
Aim for 80%+ code coverage for Application and Domain layers.
Pull Request Guidelines
Target the develop branch by default.
Use descriptive PR titles and summaries.
Reference related issues with #issue-number.
Every PR must include:
A summary of changes.
Unit/integration tests for new or updated features.
Updates to documentation (if API surface changes).
Ensure build passes and tests succeed before review.
Dependency Management
Use NuGet for all dependencies.
Keep dependencies up to date; avoid unnecessary packages.
All new dependencies must be reviewed.
Documentation
Keep README.md up to date.
All public APIs must include XML doc comments.
Add/Update OpenAPI/Swagger documentation for endpoints.
Known Limitations / TODOs
Some legacy modules (listed in /docs/LEGACY.md) may not conform to the latest architecture—do not refactor unless specifically assigned.
Multi-tenancy and distributed transactions not fully implemented.
Security
Do not log sensitive data.
Use built-in ASP.NET Core Identity for authentication if possible.
Validate all user input and use Data Annotations or FluentValidation.
Examples of Good PRs
PR #42: Add Order Processing Use Case
PR #57: Refactor Customer Aggregate
Other Notes for Codex
Prioritize code readability, maintainability, and testability.
When in doubt, add XML comments.
Prefer explicit over implicit logic.
## Project Components

### OutlineMCP (Main Project)
The main project implements the MCP server functionality using the official MCP C# SDK:

- **Program.cs** - Main entry point using the MCP SDK with stdio transport
- **OutlineTools.cs** - Static tool class containing all MCP tools for Outline API interactions

### OutlineMCP.Tests
Contains unit tests for the MCP server components, focusing on:
- Tool behavior verification
- Error handling
- API interaction patterns
- Parameter validation

## Best Practices for C# MCP Server Development

### 1. Project Setup
```csharp
// Always use nullable reference types
#nullable enable

// Use top-level statements for Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
```

### 2. Configuration Management
- Use environment variables for sensitive data
- Provide template configuration files
- Never commit actual configuration files with secrets

```csharp
// Example configuration pattern
public class OutlineService
{
    private readonly string _apiToken = Environment.GetEnvironmentVariable("OUTLINE_API_TOKEN")
        ?? throw new InvalidOperationException("OUTLINE_API_TOKEN is required");
}
```

### 3. Error Handling
- Always return proper MCP error responses
- Include detailed error messages
- Use consistent error types

```csharp
public class McpError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

// Usage
return new McpResponse<T>
{
    Error = new McpError
    {
        Code = "AUTH_ERROR",
        Message = "API token not configured"
    }
};
```

### 4. Tool Implementation
- Each tool should have a clear, single responsibility
- Use strongly-typed parameters
- Validate all inputs
- Document expected inputs and outputs

```csharp
public class ToolCallParams
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

// Implementation example
public async Task<string> ExecuteTool(ToolCallParams parameters)
{
    ValidateParameters(parameters);
    // Tool-specific logic
}
```

### 5. Testing
- Write comprehensive unit tests
- Mock external dependencies
- Test error conditions
- Verify parameter validation

```csharp
[Fact]
public async Task ToolName_Scenario_ExpectedBehavior()
{
    // Arrange
    var parameters = new Dictionary<string, object>
    {
        ["key"] = "value"
    };

    // Act
    var result = await _service.ExecuteTool(parameters);

    // Assert
    Assert.NotNull(result);
}
```

### 6. Dependency Injection
- Use DI for services and configuration
- Avoid static classes
- Make dependencies explicit

```csharp
public class OutlineService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OutlineService> _logger;

    public OutlineService(HttpClient httpClient, ILogger<OutlineService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
}
```

### 7. Logging
- Use structured logging
- Include correlation IDs
- Log appropriate detail levels

```csharp
_logger.LogInformation(
    "Executing tool {ToolName} with parameters {@Parameters}",
    toolName,
    parameters
);
```

### 8. API Communication
- Use typed HTTP clients
- Handle rate limiting
- Implement retry policies
- Validate responses

```csharp
public class OutlineService
{
    private async Task<T> SendRequest<T>(string endpoint, object data)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("Invalid response");
    }
}
```

### 9. Security Best Practices
- Validate all inputs
- Sanitize outputs
- Use secure communication
- Handle secrets properly

```csharp
// Example of input validation
public void ValidateParameters(Dictionary<string, object> parameters)
{
    if (!parameters.ContainsKey("required_param"))
        throw new ArgumentException("Missing required parameter");
}
```

### 10. Performance Considerations
- Use async/await properly
- Implement caching where appropriate
- Monitor resource usage
- Handle concurrent requests

```csharp
// Example of proper async implementation
public async Task<string> SearchDocuments(Dictionary<string, object> args)
{
    using var response = await _httpClient.PostAsJsonAsync("api/documents.search", args);
    return await response.Content.ReadAsStringAsync();
}
```

## Common Pitfalls to Avoid

1. **Improper Error Handling**
   - Don't swallow exceptions
   - Always provide meaningful error messages
   - Use appropriate error types

2. **Configuration Issues**
   - Don't hardcode credentials
   - Don't commit sensitive data
   - Always validate configuration at startup

3. **Resource Management**
   - Properly dispose of resources
   - Use `using` statements
   - Handle connection cleanup

4. **Testing Gaps**
   - Don't skip error case testing
   - Don't use production endpoints in tests
   - Don't ignore edge cases

## Getting Started

1. Clone the repository
2. Copy `appsettings.json.template` to `appsettings.json`
3. Set required environment variables
4. Run `dotnet build`
5. Run `dotnet test` to verify setup

### Running the Server

The MCP server uses the official MCP C# SDK and supports both stdio (for Cursor) and HTTP/SSE transports.

```bash
dotnet run
```

The server will start with the following capabilities:
- **Stdio transport**: For integration with Cursor and other MCP clients
- **HTTP transport**: Available on `http://localhost:5000` for testing with Postman
- **SSE support**: Server-Sent Events for real-time communication

### Testing with Postman

For HTTP/SSE testing with Postman:
1. **Method**: GET
2. **URL**: `http://localhost:5000/mcp`
3. **Headers**:
   - `Accept: text/event-stream` (for SSE)
   - `Cache-Control: no-cache`

### Testing SSE with Postman

1. **Method**: GET
2. **URL**: `http://localhost:5000/mcp/sse`
3. **Headers**:
   - `Accept: text/event-stream`
   - `Cache-Control: no-cache`
4. **Connection**: The connection will remain open and stream events

### Testing with Browser

Use the provided `test_sse.html` file to test SSE functionality in a web browser:
```bash
# Run the test script
.\test_sse.ps1
```

## Contributing

When contributing to the MCP server:

1. Follow the existing code style
2. Add tests for new functionality
3. Update documentation
4. Use meaningful commit messages
5. Create detailed pull requests 