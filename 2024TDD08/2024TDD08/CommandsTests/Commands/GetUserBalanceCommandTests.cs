using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Commands;
using Moq;
using Moq.Protected;
using Xunit;

namespace ConsoleApp.Tests.Commands
{
    public class GetUserBalanceCommandTests
    {
        private const string BaseUri = "https://localhost:7249/";

        [Fact]
        public async Task ExecuteAsync_PrintsBalance_WhenResponseIsSuccessful()
        {
            // Arrange
            var userId = 1;
            var expectedBalance = "1000.50";
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedBalance)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new GetUserBalanceCommand(httpClient, userId);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/User/{userId}/balance")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_PrintsErrorMessage_WhenResponseFails()
        {
            // Arrange
            var userId = 1;
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUri)
            };
            var command = new GetUserBalanceCommand(httpClient, userId);

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == new Uri($"{BaseUri}api/User/{userId}/balance")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
