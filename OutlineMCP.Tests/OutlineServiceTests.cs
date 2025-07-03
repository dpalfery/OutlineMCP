using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;
using OutlineMCP.Services;
using OutlineMCP.Settings;

namespace OutlineMCP.Tests;

public class OutlineToolsTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly OutlineTools _outlineTools;

    public OutlineToolsTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);
        
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");
        Environment.SetEnvironmentVariable("OUTLINE_BASE_URL", "https://test.example.com");
        
        _outlineTools = new OutlineTools(_httpClient, new OutlineSettings());
    }

    private void SetupMockResponse(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent)
            });
    }

    [Fact]
    public void SearchDocuments_WithoutApiToken_ReturnsError()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", null);

        // Act
        var result = _outlineTools.SearchDocuments("test query").Result;

        // Assert
        Assert.Contains("API token is not set", result);
    }

    [Fact]
    public async Task SearchDocuments_ValidQuery_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"results\": []}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.SearchDocuments("test query", 5);

        // Assert
        Assert.Contains("Search results for 'test query'", result);
        Assert.Contains(expectedResponse, result);
    }

    [Fact]
    public async Task SearchWiki_ValidQuery_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"results\": []}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.SearchWiki("test query", 10);

        // Assert
        Assert.Contains("Wiki search results for 'test query'", result);
        Assert.Contains(expectedResponse, result);
    }

    // These tests are removed since the methods don't exist in the OutlineTools class
    /*
    [Fact]
    public async Task SearchChangeTracker_ValidQuery_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"results\": []}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.SearchChangeTracker("test query", 10);

        // Assert
        Assert.Contains("Change Tracker search results for 'test query'", result);
        Assert.Contains(expectedResponse, result);
    }

    [Fact]
    public async Task SearchPasswordSecure_ValidQuery_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"results\": []}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.SearchPasswordSecure("test query", 10);

        // Assert
        Assert.Contains("Password Secure search results for 'test query'", result);
        Assert.Contains(expectedResponse, result);
    }

    [Fact]
    public async Task SearchPrivilegedSecure_ValidQuery_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"results\": []}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.SearchPrivilegedSecure("test query", 10);

        // Assert
        Assert.Contains("Privileged Secure search results for 'test query'", result);
        Assert.Contains(expectedResponse, result);
    }
    */

    [Fact]
    public async Task GetDocument_ValidId_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"document\": {}}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.GetDocument("doc-123");

        // Assert
        Assert.Contains("Document doc-123", result);
        Assert.Contains(expectedResponse, result);
    }

    [Fact]
    public async Task ListDocuments_ValidParameters_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"documents\": []}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.ListDocuments(20, "updatedAt", true);

        // Assert
        Assert.Contains("Documents list", result);
        Assert.Contains(expectedResponse, result);
    }

    [Fact]
    public async Task ListCollections_ValidLimit_ReturnsExpectedResult()
    {
        // Arrange
        const string expectedResponse = "{\"collections\": []}";
        SetupMockResponse(expectedResponse);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.ListCollections(15);

        // Assert
        Assert.Contains("Collections list", result);
        Assert.Contains(expectedResponse, result);
    }

    [Fact]
    public async Task SearchDocuments_HttpError_ReturnsErrorMessage()
    {
        // Arrange
        SetupMockResponse("", HttpStatusCode.InternalServerError);
        Environment.SetEnvironmentVariable("OUTLINE_API_TOKEN", "test-token");

        // Act
        var result = await _outlineTools.SearchDocuments("test query");

        // Assert
        Assert.Contains("Error searching documents", result);
    }
}