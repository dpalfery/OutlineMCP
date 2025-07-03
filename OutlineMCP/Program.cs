using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using ModelContextProtocol.AspNetCore;
using OutlineMCP.Services;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure logging for better integration with MCP clients
builder.Logging
    .AddConsole()
    .AddDebug()
    .SetMinimumLevel(LogLevel.Information);

// Register Outline services
builder.Services.AddOutlineServices(builder.Configuration);

// Add MCP server with SSE transport
builder.Services
    .AddMcpServer()
    .WithHttpTransport()    
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly();

var app = builder.Build();

app.MapMcp();

app.Run("http://localhost:3001");

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello {message}";
}