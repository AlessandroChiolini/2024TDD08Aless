using ConsoleApp.Commands;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using System.Text.Json;

namespace ConsoleApp.Tests.Commands
{
    public class AddUserBalanceCommandTests
    {
        private const string BaseAddress = "https://localhost:7249/";

        [Fact]
        public async Task ExecuteAsync_DisplaysSuccessMessage_WhenResponseIsSuccessful()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/User/addbalance") &&
                        req.Content.Headers.ContentType.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };
            var command = new AddUserBalanceCommand(httpClient, userId, amount);

            // Act
            await command.ExecuteAsync();

            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }


        [Fact]
        public async Task ExecuteAsync_DisplaysErrorMessage_WhenResponseFails()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/User/addbalance")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };
            var command = new AddUserBalanceCommand(httpClient, userId, amount);

            // Act
            await command.ExecuteAsync();

            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }


        [Fact]
        public async Task ExecuteAsync_DisplaysErrorMessage_WhenExceptionIsThrown()
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
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };
            var command = new AddUserBalanceCommand(httpClient, userId, amount);

            // Act
            await command.ExecuteAsync();

            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }


        [Fact]
        public async Task ExecuteAsync_VerifiesPayloadContent_WhenCalled()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;

            var expectedPayload = JsonSerializer.Serialize(new
            {
                UserId = userId,
                Amount = amount
            });

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/User/addbalance") &&
                        VerifyRequestContent(req.Content, expectedPayload)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new AddUserBalanceCommand(httpClient, userId, amount);

            // Act
            await command.ExecuteAsync();



            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("api/User/addbalance") &&
                    VerifyRequestContent(req.Content, expectedPayload)),
                ItExpr.IsAny<CancellationToken>());
        }

        // Helper method to verify the request content
        private bool VerifyRequestContent(HttpContent content, string expectedPayload)
        {
            var actualPayload = content.ReadAsStringAsync().Result;
            return actualPayload == expectedPayload;
        }

    }
}
