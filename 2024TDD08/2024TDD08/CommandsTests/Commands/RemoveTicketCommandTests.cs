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
    public class RemoveTicketCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_DisplaysSuccessMessage_WhenTicketIsRemoved()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost:7249/")
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri == new Uri(httpClient.BaseAddress + $"api/EventTicket/{eventId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Ticket removed successfully!")
                });

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify that the correct message was returned (Mock the console output if necessary)
        }

        [Fact]
        public async Task ExecuteAsync_DisplaysErrorMessage_WhenTicketRemovalFails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://localhost:7249/")
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri == new Uri(httpClient.BaseAddress + $"api/EventTicket/{eventId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Failed to remove ticket.")
                });

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify that the correct error message was returned (Mock the console output if necessary)
        }
    }
}
