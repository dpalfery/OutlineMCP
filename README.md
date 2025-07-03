# Outline MCP Server

A C# Model Context Protocol (MCP) server that integrates with Outline API to help Cursor consume documentation from Outline wikis. This server is **read-only** and designed specifically for documentation consumption.

## Features

- **Outline Integration**: Connects to Outline via REST API
- **MCP Protocol Support**: Implements the Model Context Protocol for seamless integration with Cursor
- **Read-Only Operations**: Only supports reading and searching documents (no write operations)
- **Web-Based Server**: Runs as an HTTP server on localhost:3001
- **Documentation Access**: Retrieves and converts Outline documents to markdown format
- **Search Capabilities**: Search through Outline documents using the Outline search API
- **Documentation Focus**: Specialized tools for searching and accessing documentation

## Prerequisites

- .NET 8.0 SDK or later
- Outline instance with API access
- API token for authentication
- Cursor IDE (for MCP integration)

## Setup

### 1. Clone and Build

```bash
git clone <repository-url>
cd OutlineMCP
dotnet build
```

### 2. Environment Configuration

Set the following environment variables:

```bash
# Windows
set OUTLINE_BASE_URL=https://your-outline-instance.com
set OUTLINE_API_TOKEN=your-api-token

# Linux/Mac
export OUTLINE_BASE_URL=https://your-outline-instance.com
export OUTLINE_API_TOKEN=your-api-token
```

**Note**: For local development, you can copy `appsettings.json.template` to `appsettings.json` and modify the settings as needed. The `appsettings.json` file is ignored by git for security reasons.

### 3. Get Outline API Token

1. Go to your Outline instance
2. Log in with your account
3. Navigate to Settings → API Tokens
4. Click "Create API token"
5. Give it a name (e.g., "MCP Server")
6. Copy the generated token

## Running the MCP Server

**Note**: This MCP server runs as an HTTP server on localhost:3001. Cursor connects to it via HTTP, not by executing the server directly.

### Option 1: Development Mode

```bash
# Start the server in development mode
dotnet run
```

The server will start and listen for MCP requests on `http://localhost:3001/mcp`.

### Option 2: Production Mode

```bash
# Build for production
dotnet publish -c Release

# Run the published application
cd bin/Release/net8.0/publish
./OutlineMCP
```

### Option 3: As a Windows Service

```bash
# Install as a Windows service (requires admin privileges)
sc create "OutlineMCP" binPath="C:\path\to\OutlineMCP.exe"
sc start "OutlineMCP"
```

## Configuring Cursor with the MCP Server

### 1. Install Cursor IDE

Download and install Cursor from [https://cursor.sh](https://cursor.sh)

### 2. Setup Environment Variables

Set your environment variables before starting the MCP server:

```bash
# Windows
set OUTLINE_BASE_URL=https://your-outline-instance.com
set OUTLINE_API_TOKEN=your-api-token

# Linux/Mac
export OUTLINE_BASE_URL=https://your-outline-instance.com
export OUTLINE_API_TOKEN=your-api-token
```

**Note**: These environment variables must be set when starting the MCP server, not in the Cursor configuration.

### 3. Configure MCP Server in Cursor

**Important**: This MCP server uses **HTTP transport** and runs as a web server on localhost:3001. Cursor connects to it via HTTP.

1. **Create MCP Configuration File**:
   - **Project-specific**: Create `.cursor/mcp.json` in your project directory
   - **Global**: Create `~/.cursor/mcp.json` in your home directory

2. **Add MCP Server Configuration**:

```json
{
 "mcpServers": {
    "OutlineWiki": {
      "url": "http://localhost:3001"
    }
  }
}
```

**Note**: Make sure the MCP server is running on localhost:3001 before configuring Cursor.

### 4. Starting the HTTP Server

The MCP server runs as an HTTP server. You need to start it before configuring Cursor:

1. **Start the HTTP server**:
   ```bash
   dotnet run
   ```

The server will start and listen for MCP requests on `http://localhost:3001`.

### 5. Restart Cursor

After configuring the MCP server, restart Cursor for the changes to take effect.

### 6. Verify Connection

1. Ensure the MCP server is running on localhost:3001
2. Open Cursor and check that the MCP server appears in the available tools
3. You can test the connection by asking questions about documentation

## Using the MCP Server in Cursor

### 1. Verify Connection

Once configured, you should see the MCP server available in Cursor. You can verify this by:

1. Opening the Command Palette (`Ctrl+Shift+P`)
2. Typing "MCP" to see available MCP commands
3. The server should appear as "outline-mcp"

### 2. Query Examples

You can now ask Cursor questions about documentation:

```
"Search for installation guides"
"Find configuration documentation"
"Look up troubleshooting steps"
"Search the wiki for best practices"
```

### 3. Available Tools in Cursor

The following tools are available through the MCP server:

| Tool | Description | Usage |
|------|-------------|-------|
| `search_documents` | General search across all documentation | "Search for installation guides" |
| `searchWiki` | Search the knowledge base for specific terms | "Search wiki for troubleshooting" |
| `search_specific_app` | Application specific search (optionally accepts collectionId) | "Search application documentation" |
| `get_document` | Get specific document by ID | "Get document with ID abc123" |
| `list_collections` | List available collections | "Show all documentation collections" |
| `list_documents` | List documents in a collection | "List documents in collection" |

## MCP Protocol Support

This server implements the following MCP methods:

- `initialize`: Handles server initialization
- `tools/list`: Lists available tools (read-only operations)
- `tools/call`: Executes read-only tools
- `resources/list`: Lists available resources
- `resources/read`: Reads resource content

### Available Tools

| Tool | Description | Parameters |
|------|-------------|------------|
| `search_documents` | Search for application documentation and guides in the knowledge base | `query`, `collectionId` (optional), `limit` (optional) |
| `searchWiki` | Search the knowledge base for specific terms across all application documentation | `term`, `limit` (optional) |
| `search_specific_app` | Search specifically for application documentation and guides | `query`, `limit` (optional) |
| `get_document` | Get a specific application document by ID | `id` |
| `list_collections` | List available application documentation collections | `limit` (optional) |
| `list_documents` | List documents in a specific application collection | `collectionId`, `limit` (optional) |

### Available Resources

| Resource | Description |
|----------|-------------|
| `outline://collections` | List of all accessible application documentation collections |
| `outline://recent-documents` | Recently updated application documentation and guides |
| `outline://starred-documents` | Important application documentation marked as starred |

## Configuration

The server uses environment variables for configuration:

| Variable | Description | Required |
|----------|-------------|----------|
| `OUTLINE_BASE_URL` | Your Outline instance URL | Yes |
| `OUTLINE_API_TOKEN` | API token for authentication | Yes |

## Error Handling

The server includes comprehensive error handling:

- **Authentication Errors**: Invalid API token
- **Network Errors**: Connection issues with Outline API
- **Parse Errors**: Invalid JSON-RPC requests
- **Resource Errors**: Document not found or access denied

## Development

### Project Structure

```
OutlineMCP/
├── Services/
│   └── OutlineService.cs        # Outline API integration (read-only)
├── Models/
│   ├── McpRequest.cs            # MCP request model
│   ├── McpResponse.cs           # MCP response model
│   ├── McpError.cs              # MCP error model
│   ├── ToolCallParams.cs        # Tool call parameters model
│   └── ResourceReadParams.cs    # Resource read parameters model
├── Program.cs                   # Web application entry point
├── appsettings.json.template    # Configuration template
└── OutlineMCP.csproj           # Project file
```

### Adding New Features

1. **New Tools**: Add to the switch statement in `HandleToolsCall`
2. **New Resources**: Add to the switch statement in `HandleResourcesRead`
3. **API Endpoints**: Extend `OutlineService` with new read-only methods

## Security

This server is designed to be **read-only** and does not support any write operations to your Outline instance. This ensures that:

- No documents can be created, updated, or deleted
- No collections can be modified
- The server is safe to use in production environments

## Troubleshooting

### Common Issues

1. **Authentication Failed**
   - Verify your API token is correct
   - Ensure the token has the necessary read permissions
   - Check that the Outline URL is correct

2. **Server Not Starting**
   - Check that port 3001 is available
   - Verify environment variables are set correctly
   - Ensure .NET 8.0 SDK is installed

3. **Network Errors**
   - Check your internet connection
   - Verify the Outline URL is correct
   - Ensure your firewall allows HTTPS connections

4. **Cursor MCP Integration Issues**

   **MCP Server Not Appearing in Cursor:**
   - Verify the MCP server is running on localhost:3001
   - Check that the HTTP server is accessible at `http://localhost:3001/mcp`
   - Verify the Cursor configuration uses the correct HTTP endpoint
   - Restart Cursor after configuration changes
   - Check Cursor's developer console for errors

   **Authentication Errors in Cursor:**
   - Ensure the API token is correctly set in the MCP server environment variables
   - Verify the token has not expired
   - Check that the environment variables are properly configured when starting the server

   **Server Connection Timeout:**
   - Verify the server is running on localhost:3001
   - Check that port 3001 is not blocked by firewall
   - Ensure the HTTP endpoint `http://localhost:3001/mcp` is accessible
   - Test the endpoint manually with curl or browser

5. **Tool Execution Errors**
   - Check server logs for detailed error messages
   - Verify the Outline API is accessible
   - Ensure the search queries are properly formatted

### Debugging MCP Server

1. **Enable Verbose Logging**:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "Microsoft": "Information"
       }
     }
   }
   ```

2. **Test Server Manually**:
   ```bash
   curl -X POST http://localhost:3001/mcp \
     -H "Content-Type: application/json" \
     -d '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}'
   ```

3. **Check Cursor Logs**:
   - Open Command Palette (`Ctrl+Shift+P`)
   - Type "Developer: Toggle Developer Tools"
   - Check the Console tab for MCP-related errors

### Logging

The server uses structured logging. Set the log level in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes (read-only operations only)
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
1. Check the troubleshooting section
2. Review the logs for error details
3. Open an issue on the repository
