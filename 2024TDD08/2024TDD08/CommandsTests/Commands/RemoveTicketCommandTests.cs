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
        public async Task ExecuteAsync_DisplaysError_WhenEventIdIsEmpty()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            Console.SetIn(new System.IO.StringReader("")); // Simulate empty Event ID

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
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

            // Simulate an exception when fetching tickets
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Internal Server Error")
                });

            // Simulate valid Event ID input to ensure the flow proceeds to fetch tickets
            Console.SetIn(new System.IO.StringReader("E1"));

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify that the GET request was made to fetch tickets
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                ItExpr.IsAny<CancellationToken>());
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
                        req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]") // No tickets
                });

            Console.SetIn(new System.IO.StringReader("E1"));

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_RemovesTicket_WhenValidEventIdProvided()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<UserTicketDto>
                    {
                        new UserTicketDto { TicketId = 1, EventId = eventId, EventName = "Concert", Quantity = 1 }
                    }))
                });

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri.ToString().Contains($"api/EventTicket/event/{eventId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Ticket removed successfully")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            Console.SetIn(new System.IO.StringReader(eventId));

            // Act
            await command.ExecuteAsync();

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri.ToString().Contains($"api/EventTicket/event/{eventId}")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_HandlesException_WhenDeletingTicketThrows()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";

            // Mock the GET request to fetch tickets successfully
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<UserTicketDto>
                    {
                new UserTicketDto { TicketId = 1, EventId = eventId, EventName = "Concert", Quantity = 1 }
                    }))
                });

            // Mock the DELETE request to throw an exception
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri.ToString().Contains($"api/EventTicket/event/{eventId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network failure during ticket removal"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            // Simulate valid Event ID input
            Console.SetIn(new System.IO.StringReader(eventId));

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify that the DELETE request was made for the valid Event ID
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri.ToString().Contains($"api/EventTicket/event/{eventId}")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_DisplaysError_WhenDeleteRequestFails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var userId = 1;
            var eventId = "E1";

            // Mock the GET request to fetch tickets successfully
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<UserTicketDto>
                    {
                new UserTicketDto { TicketId = 1, EventId = eventId, EventName = "Concert", Quantity = 1 }
                    }))
                });

            // Mock the DELETE request to fail
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri.ToString().Contains($"api/EventTicket/event/{eventId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Failed to remove ticket.")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            var command = new RemoveTicketCommand(httpClient, userId);

            // Simulate valid Event ID input
            Console.SetIn(new System.IO.StringReader(eventId));

            // Act
            await command.ExecuteAsync();

            // Assert
            // Verify that the DELETE request was made
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri.ToString().Contains($"api/EventTicket/event/{eventId}")),
                ItExpr.IsAny<CancellationToken>());

            // Assert that the GET request was made to fetch tickets
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains($"api/EventTicket/user/{userId}/tickets")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
