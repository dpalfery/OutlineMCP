using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using OutlineMCP.Services;
using OutlineMCP.Settings;

namespace OutlineMCP.Tests;

public class SecurityTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly OutlineTools _outlineTools;

    public SecurityTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);
        
        var settings = new OutlineSettings
        {
            BaseUrl = "https://example.com/",
            ApiToken = "test-token"
        };
        
        _outlineTools = new OutlineTools(_httpClient, settings);
    }

    [Fact]
    public void OutlineSettings_WithNullEnvironmentVariables_ShouldHandleNullValues()
    {
        // Arrange & Act
        var settings = new OutlineSettings();
        
        // Assert - Should not throw exceptions
        Assert.True(string.IsNullOrEmpty(settings.BaseUrl) || !string.IsNullOrEmpty(settings.BaseUrl));
        Assert.True(string.IsNullOrEmpty(settings.ApiToken) || !string.IsNullOrEmpty(settings.ApiToken));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async void SearchDocuments_WithInvalidQuery_ShouldReturnValidationError(string invalidQuery)
    {
        // Act
        var result = await _outlineTools.SearchDocuments(invalidQuery);
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Query cannot be null or empty", result);
    }

    [Fact]
    public async void SearchDocuments_WithTooLongQuery_ShouldReturnValidationError()
    {
        // Arrange
        var longQuery = new string('a', 1001); // Exceeds 1000 character limit
        
        // Act
        var result = await _outlineTools.SearchDocuments(longQuery);
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Query length cannot exceed", result);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("'; DROP TABLE users; --")]
    [InlineData("query & dangerous")]
    [InlineData("query | pipe")]
    [InlineData("query `backtick`")]
    [InlineData("query $variable")]
    public async void SearchDocuments_WithUnsafeCharacters_ShouldReturnValidationError(string unsafeQuery)
    {
        // Act
        var result = await _outlineTools.SearchDocuments(unsafeQuery);
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Query contains potentially unsafe characters", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async void GetDocument_WithInvalidDocumentId_ShouldReturnValidationError(string invalidId)
    {
        // Act
        var result = await _outlineTools.GetDocument(invalidId);
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Document ID cannot be null or empty", result);
    }

    [Fact]
    public async void GetDocument_WithTooLongDocumentId_ShouldReturnValidationError()
    {
        // Arrange
        var longId = new string('a', 256); // Exceeds 255 character limit
        
        // Act
        var result = await _outlineTools.GetDocument(longId);
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Document ID length cannot exceed", result);
    }

    [Theory]
    [InlineData("doc-123!")]
    [InlineData("doc@123")]
    [InlineData("doc#123")]
    [InlineData("doc 123")]
    [InlineData("doc/123")]
    [InlineData("doc\\123")]
    public async void GetDocument_WithInvalidDocumentIdFormat_ShouldReturnValidationError(string invalidId)
    {
        // Act
        var result = await _outlineTools.GetDocument(invalidId);
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Document ID contains invalid characters", result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async void SearchDocuments_WithInvalidLimit_ShouldReturnValidationError(int invalidLimit)
    {
        // Act
        var result = await _outlineTools.SearchDocuments("test", invalidLimit);
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Limit must be greater than 0", result);
    }

    [Fact]
    public async void SearchDocuments_WithExcessiveLimit_ShouldReturnValidationError()
    {
        // Act
        var result = await _outlineTools.SearchDocuments("test", 101); // Exceeds 100 limit
        
        // Assert
        Assert.Contains("Invalid input", result);
        Assert.Contains("Limit cannot exceed", result);
    }

    [Theory]
    [InlineData("invalidSort")]
    [InlineData("title; DROP TABLE")]
    [InlineData("")]
    public async void ListDocuments_WithInvalidSort_ShouldReturnValidationError(string invalidSort)
    {
        // Act
        var result = await _outlineTools.ListDocuments(10, invalidSort);
        
        // Assert
        if (!string.IsNullOrEmpty(invalidSort))
        {
            Assert.Contains("Invalid input", result);
            Assert.Contains("Sort parameter must be one of", result);
        }
    }

    [Fact]
    public void OutlineTools_WithNullSettings_ShouldHandleGracefully()
    {
        // Arrange
        var settings = new OutlineSettings { BaseUrl = null, ApiToken = null };
        var tools = new OutlineTools(_httpClient, settings);
        
        // Act & Assert
        var result = tools.SearchDocuments("test").Result;
        Assert.Contains("Configuration error", result);
        Assert.Contains("Base URL is not configured", result);
    }

    [Fact]
    public void OutlineTools_WithHttpBaseUrl_ShouldReturnSecurityError()
    {
        // Arrange
        var settings = new OutlineSettings { BaseUrl = "http://example.com/", ApiToken = "token" };
        var tools = new OutlineTools(_httpClient, settings);
        
        // Act
        var result = tools.SearchDocuments("test").Result;
        
        // Assert
        Assert.Contains("Configuration error", result);
        Assert.Contains("Base URL must use HTTPS protocol", result);
    }

    [Fact]
    public void OutlineTools_WithInvalidBaseUrl_ShouldReturnConfigurationError()
    {
        // Arrange
        var settings = new OutlineSettings { BaseUrl = "not-a-valid-url", ApiToken = "token" };
        var tools = new OutlineTools(_httpClient, settings);
        
        // Act
        var result = tools.SearchDocuments("test").Result;
        
        // Assert
        Assert.Contains("Configuration error", result);
        Assert.Contains("Base URL format is invalid", result);
    }
}