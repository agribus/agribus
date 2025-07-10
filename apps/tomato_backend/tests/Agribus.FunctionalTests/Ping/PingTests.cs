using Agribus.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Agribus.UnitTests.Ping;

public class PingTests
{
    private class TestLogger<T> : ILogger<T>
    {
        public List<(LogLevel LogLevel, string Message)> LoggedMessages { get; } = new();

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            LoggedMessages.Add((logLevel, formatter(state, exception)));
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;
    }

    private readonly PingController _controller;
    private readonly TestLogger<PingController> _logger;

    public PingTests()
    {
        _logger = new TestLogger<PingController>();
        _controller = new PingController(_logger);
    }

    [Fact]
    public Task Should_ReturnPong()
    {
        // Act
        var result = _controller.Ping();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("pong", okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Should_LogInformation_WhenPingReceived()
    {
        // Act
        _controller.Ping();

        // Assert
        Assert.Contains(
            _logger.LoggedMessages,
            msg => msg.LogLevel == LogLevel.Information && msg.Message == "Ping received"
        );
        return Task.CompletedTask;
    }
}
