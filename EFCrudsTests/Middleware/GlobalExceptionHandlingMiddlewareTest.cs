using System.Text.Json;
using EFCruds.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EFCrudsTests.Middleware
{
    public class GlobalExceptionHandlingMiddlewareTest
    {
        [Fact]
        public async Task InvokeAsync_Success_Test()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var httpContextMock = new DefaultHttpContext();
            RequestDelegate requestDelgateMock = async (ctx) => await Task.CompletedTask;

            //Act
            var sut = new GlobalExceptionHandlingMiddleware(loggerMock.Object);
            await sut.InvokeAsync(httpContextMock, requestDelgateMock);

            // Assert
            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never // Verify LogError() is not called
            );
            
        }

        [Fact]
        public async Task InvokeAsync_Failure_Test()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var httpContextMock = new DefaultHttpContext();

            RequestDelegate requestDelgateMock = async (ctx) => { await Task.CompletedTask; throw new Exception(); };

            //Act
            var sut = new GlobalExceptionHandlingMiddleware(loggerMock.Object);
            await sut.InvokeAsync(httpContextMock, requestDelgateMock);

            // Assert
            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once // Verify LogError() is called
            );

            Assert.Equal(500, httpContextMock.Response.StatusCode);
            
        }
    }
}