using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
        private const string BaseAddress = "https://localhost:7249/";

        [Fact]
        public async Task ExecuteAsync_DisplaysSuccessMessage_WhenTicketIsRemoved()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri == new Uri(httpClient.BaseAddress + $"api/EventTicket/event/{eventId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"Message\":\"Ticket removed successfully!\"}")
                });

            // Act
            await command.ExecuteAsync();

            // Assert
            // Add assertions to mock Console output if needed
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
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri == new Uri(httpClient.BaseAddress + $"api/EventTicket/event/{eventId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{\"Message\":\"Failed to remove ticket.\"}")
                });

            // Act
            await command.ExecuteAsync();

            // Assert
            // Add assertions to mock Console output if needed
        }

        [Fact]
        public async Task ExecuteAsync_DisplaysError_WhenFetchingTicketsFails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri(httpClient.BaseAddress + $"api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Internal Server Error")
                });

            // Act
            await command.ExecuteAsync();

            // Assert
            // Add assertions to mock Console output if needed
        }

        [Fact]
        public async Task ExecuteAsync_DisplaysWarning_WhenNoTicketsFound()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri == new Uri(httpClient.BaseAddress + $"api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<UserTicketDto>()))
                });

            // Act
            await command.ExecuteAsync();

            // Assert
            // Add assertions to mock Console output if needed
        }

        [Fact]
        public async Task ExecuteAsync_DisplaysError_WhenNetworkFails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network failure"));

            // Act
            await command.ExecuteAsync();

            // Assert
            // Add assertions to mock Console output if needed
        }
    }
}
