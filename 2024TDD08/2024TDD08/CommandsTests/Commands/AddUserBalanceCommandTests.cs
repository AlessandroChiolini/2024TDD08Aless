using ConsoleApp.Commands;
using System.Net;
using Moq;
using Xunit;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq.Protected;

namespace ConsoleApp.Tests.Commands
{
    public class AddUserBalanceCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_SuccessfulResponse_DisplaysSuccessMessage()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK);
            var command = new AddUserBalanceCommand(httpClient, userId, amount);

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify console output or logs (if logging is added)
        }

        [Fact]
        public async Task ExecuteAsync_FailedResponse_DisplaysErrorMessage()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;
            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest);
            var command = new AddUserBalanceCommand(httpClient, userId, amount);

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify console output or logs (if logging is added)
        }

        [Fact]
        public async Task ExecuteAsync_ExceptionThrown_DisplaysErrorMessage()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock.Object);
            var command = new AddUserBalanceCommand(httpClient, userId, amount);

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify console output or logs (if logging is added)
        }

        private HttpClient CreateMockHttpClient(HttpStatusCode statusCode)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent("")
                });

            return new HttpClient(handlerMock.Object);
        }
    }
}
